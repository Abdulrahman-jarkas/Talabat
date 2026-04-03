using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.SharedKernal;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Orders.Application.CheckoutSession.IntegrationEvents;

internal class OnProductPriceChangedCancelSessionsHandler(
	ICheckoutSessionRepository checkoutSessionRepository) : INotificationHandler<ProductPriceChangedIntegrationEvent>
{
	public async Task Handle(ProductPriceChangedIntegrationEvent notification, CancellationToken cancellationToken)
	{
		var activeSessions = await checkoutSessionRepository.GetActiveSessionsByProductIdAsync(
			notification.ProductId,
			cancellationToken);

		if (activeSessions.Count == 0)
			return;

		foreach (var session in activeSessions)
		{
			var cancelResult = session.Cancel();

			if (cancelResult.IsError)
				throw new EventualConsistencyException(
					EventualConsistencyError.From(
						"PriceChanged.SessionCancellationFailed",
						$"Could not cancel session {session.Id} for Product {notification.ProductId}."),
					cancelResult.Errors);
		}

		// SaveChangesAsync dispatches CheckoutSessionCancelledEvent for each cancelled session,
		// which triggers reservation removal via OnCheckoutSessionCancelledRemoveReservationsHandler.
		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
	}
}
