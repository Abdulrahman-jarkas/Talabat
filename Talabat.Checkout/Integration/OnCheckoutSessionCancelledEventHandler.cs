using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;

namespace Talabat.Checkout.Integration;

internal class OnCheckoutSessionCancelledEventHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	IProductRepository productRepository,
	ILogger<OnCheckoutSessionCancelledEventHandler> logger) : INotificationHandler<CheckoutSessionCancelledEvent>
{
	public async Task Handle(CheckoutSessionCancelledEvent notification, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByIdAsync(notification.CheckoutSessionId, cancellationToken);
		if (session is null)
			return;

		var productIds = session.Items.Select(i => i.ProductId).ToList();
		var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

		foreach (var item in session.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null)
			{
				logger.LogWarning(
					"[SessionCancelled] Checkout Product {ProductId} not found; skipping release for session {SessionId}.",
					item.ProductId,
					session.Id);
				continue;
			}

			var releaseResult = product.Release(item.Quantity);

			if (releaseResult.IsError)
			{
				logger.LogWarning(
					"[SessionCancelled] Could not release {Quantity} units for Product {ProductId} in session {SessionId}. Errors: {Errors}",
					item.Quantity,
					item.ProductId,
					session.Id,
					string.Join(", ", releaseResult.Errors.Select(e => e.Description)));
			}
		}
	}
}
