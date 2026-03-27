using MediatR;
using Talabat.Checkout.Data.Repositories;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;

namespace Talabat.Checkout.Integration;

internal class OnPaymentFailedEventHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ISender sender) : INotificationHandler<PaymentFailedEvent>
{
	public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
	{
		var checkoutSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(
			notification.CustomerId,
			cancellationToken);

		if (checkoutSession is null)
		{
			var error = IntegrationErrors.PaymentFailed.CheckoutSessionNotFound(notification.CustomerId);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		var cancelResult = checkoutSession.Cancel();

		if (cancelResult.IsError)
		{
			var error = IntegrationErrors.PaymentFailed.FailedToCancel(
				checkoutSession.Id,
				cancelResult.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		// Release reserved stock
		var releaseItems = checkoutSession.Items
			.Select(i => new ReleaseStockItem(i.ProductId, i.Quantity))
			.ToList();

		await sender.Send(new ReleaseStockRequest(releaseItems), cancellationToken);

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
	}
}
