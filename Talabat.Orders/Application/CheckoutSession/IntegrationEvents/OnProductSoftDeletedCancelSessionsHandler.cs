using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.SharedKernal;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Orders.Application.CheckoutSession.IntegrationEvents;

internal class OnProductSoftDeletedCancelSessionsHandler(
	ICheckoutSessionRepository checkoutSessionRepository) : INotificationHandler<ProductSoftDeletedIntegrationEvent>
{
	public async Task Handle(ProductSoftDeletedIntegrationEvent notification, CancellationToken cancellationToken)
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
						"ProductDeleted.SessionCancellationFailed",
						$"Could not cancel session {session.Id} for Product {notification.ProductId}."),
					cancelResult.Errors);
		}

		using var scope = ModuleTransactionScope.Create();

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();
	}
}
