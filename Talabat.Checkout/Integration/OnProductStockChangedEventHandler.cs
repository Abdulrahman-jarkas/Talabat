using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Checkout.Integration;

internal class OnProductQuantityChangedEventHandler(
	IProductRepository productRepository,
	ICheckoutSessionRepository checkoutSessionRepository,
	ILogger<OnProductQuantityChangedEventHandler> logger) : INotificationHandler<ProductQuantityChangedIntegrationEvent>
{
	public async Task Handle(ProductQuantityChangedIntegrationEvent notification, CancellationToken cancellationToken)
	{
		var product = await productRepository.GetByIdAsync(notification.ProductId, cancellationToken);
		if (product is null)
			return;

		if (product.Quantity == notification.NewQuantity)
			return;

		var updateResult = product.UpdateQuantity(notification.NewQuantity);

		if (updateResult.IsError)
		{
			logger.LogWarning(
				"[QuantityChanged] Could not update quantity for Product {ProductId}. Errors: {Errors}",
				notification.ProductId,
				string.Join(", ", updateResult.Errors.Select(e => e.Description)));
			return;
		}

		// If quantity decreased below reservations, expire affected sessions
		if (product.AvailableQuantity < 0)
		{
			var activeSessions = await checkoutSessionRepository.GetActiveSessionsByProductIdAsync(
				notification.ProductId,
				cancellationToken);

			if (activeSessions is not null)
			{
				foreach (var session in activeSessions)
				{
					var expireResult = session.Expire();

					if (expireResult.IsError)
					{
						logger.LogWarning(
							"[QuantityChanged] Could not expire session {SessionId} for Product {ProductId}. Errors: {Errors}",
							session.Id,
							notification.ProductId,
							string.Join(", ", expireResult.Errors.Select(e => e.Description)));
					}
				}
			}
		}

		await productRepository.SaveChangesAsync(cancellationToken);
	}
}
