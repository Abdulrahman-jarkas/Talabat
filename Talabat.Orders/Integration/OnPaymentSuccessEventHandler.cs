using Ardalis.GuardClauses;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate;
using Talabat.Payments.Contracts;
using Talabat.Users.Contracts;

namespace Talabat.Orders.Integration;

internal class OnPaymentSuccessEventHandler(
	IOrdersRepository ordersRepository,
	ISender sender) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		// Get checkout session details from Users module using customer ID from event
		var checkoutSession = await sender.Send(
			new ActiveCheckoutSessionQuery(notification.CustomerId),
			cancellationToken);

		if (checkoutSession is null)
			throw new NotFoundException(nameof(checkoutSession),
				$"Checkout session not found for customer {notification.CustomerId}");

		// Validate that address is set
		if (!checkoutSession.AddressId.HasValue)
			throw new InvalidOperationException("Address must be set for checkout session");

		// Create the order with payment
		var payment = Payment.Card(notification.PaymentId, PaymentStatusValues.Paid);

		var order = new Order(
			notification.CustomerId,
			checkoutSession.MerchantId,
			payment,
			checkoutSession.CheckoutSessionId,
			checkoutSession.AddressId.Value);

		// Add order items
		foreach (var item in checkoutSession.Items)
		{
			var addItemResult = order.AddItem(item.ProductId, item.Quantity, item.BasePrice);

			if (addItemResult.IsError)
				throw new InvalidOperationException(
					string.Join(",", addItemResult.Errors.Select(e => e.Description)));
		}

		// Accept the order
		var acceptResult = order.Accept();

		if (acceptResult.IsError)
			throw new InvalidOperationException(
				string.Join(",", acceptResult.Errors.Select(e => e.Description)));

		await ordersRepository.AddAsync(order, cancellationToken);
		await ordersRepository.SaveChangesAsync(cancellationToken);
	}
}
