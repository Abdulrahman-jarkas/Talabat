using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Checkout.Integration;

internal class OnProductPriceChangedEventHandler(
	IProductRepository productRepository,
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnProductPriceChangedEventHandler> logger) : INotificationHandler<ProductPriceChangedIntegrationEvent>
{
	public async Task Handle(ProductPriceChangedIntegrationEvent notification, CancellationToken cancellationToken)
	{
		var product = await productRepository.GetByIdAsync(notification.ProductId, cancellationToken);
		if (product is null)
			return;

		if (product.BasePrice == notification.NewBasePrice)
			return;

		var updateResult = product.UpdatePrice(notification.NewBasePrice);

		if (updateResult.IsError)
		{
			logger.LogWarning(
				"[PriceChanged] Could not update price for Product {ProductId}. Errors: {Errors}",
				notification.ProductId,
				string.Join(", ", updateResult.Errors.Select(e => e.Description)));
			return;
		}

		var activeSessions = await checkoutSessionRepository.GetActiveSessionsByProductIdAsync(
			notification.ProductId,
			cancellationToken);

		if (activeSessions is not null && activeSessions.Count > 0)
		{
			foreach (var session in activeSessions)
			{
				var expireResult = session.Expire();

				if (expireResult.IsError)
				{
					logger.LogWarning(
						"[PriceChanged] Could not expire session {SessionId} for Product {ProductId}. Errors: {Errors}",
						session.Id,
						notification.ProductId,
						string.Join(", ", expireResult.Errors.Select(e => e.Description)));
				}
			}
		}

		await productRepository.SaveChangesAsync(cancellationToken);
	}
}
