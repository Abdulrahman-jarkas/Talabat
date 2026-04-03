using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Checkout.Integration;

internal class OnProductSoftDeletedEventHandler(
	IProductRepository productRepository,
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnProductSoftDeletedEventHandler> logger) : INotificationHandler<ProductSoftDeletedIntegrationEvent>
{
	public async Task Handle(ProductSoftDeletedIntegrationEvent notification, CancellationToken cancellationToken)
	{
		var product = await productRepository.GetByIdAsync(notification.ProductId, cancellationToken);
		if (product is null)
			return;

		if (product.IsDeleted)
			return;

		var deleteResult = product.MarkDeleted();

		if (deleteResult.IsError)
		{
			logger.LogWarning(
				"[ProductDeleted] Could not mark Product {ProductId} as deleted. Errors: {Errors}",
				notification.ProductId,
				string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
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
						"[ProductDeleted] Could not expire session {SessionId} for Product {ProductId}. Errors: {Errors}",
						session.Id,
						notification.ProductId,
						string.Join(", ", expireResult.Errors.Select(e => e.Description)));
				}
			}
		}

		await productRepository.SaveChangesAsync(cancellationToken);
	}
}
