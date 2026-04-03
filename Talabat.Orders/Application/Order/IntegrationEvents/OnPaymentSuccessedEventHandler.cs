using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Orders.Domain.OrderAggregate;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.IntegrationEvents;

internal class OnPaymentSuccessedEventHandler(
	ISender sender,
	IOrdersRepository ordersRepository,
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnPaymentSuccessedEventHandler> logger) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		// 1. Load checkout session
		var session = await checkoutSessionRepository.GetByPaymentIdAsync(
			notification.PaymentId,
			cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentSuccess.SessionNotFound",
					$"No checkout session found for PaymentId {notification.PaymentId}."));

		// Idempotency: already completed, nothing to do
		if (session.Lifetime.StoredStatus == CheckoutSessionStatusValues.Completed)
			return;

		// 2. Complete the session
		var completeResult = session.Complete();
		if (completeResult.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentSuccess.SessionCompletionFailed",
					$"Failed to complete session {session.Id}."),
				completeResult.Errors);

		// 3. Create the order
		var payment = Payment.Create(notification.PaymentId);

		var order = new Domain.OrderAggregate.Order(
			session.CustomerId,
			session.MerchantId,
			payment,
			session.Id,
			session.AddressId);

		foreach (var item in session.Items)
		{
			var addItemResult = order.AddItem(item.ProductId, item.Quantity, item.Price);

			if (addItemResult.IsError)
				throw new EventualConsistencyException(
					EventualConsistencyError.From(
						"PaymentSuccess.OrderCreationFailed",
						$"Failed to add item {item.ProductId} to order for session {session.Id}."),
					addItemResult.Errors);
		}

		// 4. Complete session + create order + set orderId on reservations in a single transaction
		var editItems = session.Items
			.Select(i => new EditReservationItem(i.ProductId, session.Id, order.Id))
			.ToList();

		using var scope = ModuleTransactionScope.Create();

		var editResult = await sender.Send(new EditReservationRequest(editItems), cancellationToken);
		if (editResult.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentSuccess.ReservationEditFailed",
					$"Failed to set orderId on reservations for session {session.Id}."),
				editResult.Errors);

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		await ordersRepository.AddAsync(order, cancellationToken);
		await ordersRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		logger.LogInformation(
			"[PaymentSuccess] Session {SessionId} completed, Order {OrderId} created for PaymentId {PaymentId}.",
			session.Id,
			order.Id,
			notification.PaymentId);
	}
}
