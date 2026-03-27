using MediatR;
using Talabat.Checkout.Data.Repositories;
using Talabat.Orders.Contracts;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;

namespace Talabat.Checkout.Integration;

internal class OnPaymentSuccessEventHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ISender sender) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		// 1. Get the checkout session from Checkout context
		var checkoutSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(
			notification.CustomerId,
			cancellationToken);

		if (checkoutSession is null)
		{
			var error = IntegrationErrors.PaymentSuccessed.CheckoutSessionNotFound(notification.CustomerId);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		// 2. Send request to create order in Orders context
		var orderItems = checkoutSession.Items
			.Select(i => new CreateOrderItemDto(i.ProductId, i.Quantity, i.Price))
			.ToList();

		var createOrderResult = await sender.Send(
			new CreateOrderRequest(
				checkoutSession.CustomerId,
				checkoutSession.MerchantId,
				checkoutSession.Id,
				checkoutSession.AddressId,
				notification.PaymentId,
				orderItems),
			cancellationToken);

		if (createOrderResult.IsError)
		{
			var error = IntegrationErrors.PaymentSuccessed.FailedToCreateOrder(
				checkoutSession.Id,
				createOrderResult.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		// 3. Complete the checkout session with paymentId and orderId
		var completeResult = checkoutSession.Complete(
			notification.PaymentId,
			createOrderResult.Value.OrderId);

		if (completeResult.IsError)
		{
			var error = IntegrationErrors.PaymentSuccessed.FailedToComplete(
				checkoutSession.Id,
				completeResult.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		// 4. Deduct reserved stock (move from reserved to sold)
		var deductItems = checkoutSession.Items
			.Select(i => new DeductStockItem(i.ProductId, i.Quantity))
			.ToList();

		await sender.Send(new DeductStockRequest(deductItems), cancellationToken);

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
	}
}
