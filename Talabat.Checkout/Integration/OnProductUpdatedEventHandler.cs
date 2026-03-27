using MediatR;
using Talabat.Checkout.Data.Repositories;
using Talabat.Products.Contracts;
using Talabat.ProductsManagement.Contracts;

namespace Talabat.Checkout.Integration;

internal class OnProductUpdatedEventHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ISender sender) : INotificationHandler<ProductUpdatedEvent>
{
	public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
	{
		var activeSessions = await checkoutSessionRepository.GetActiveSessionsByProductIdAsync(
			notification.ProductId,
			cancellationToken);

		if (activeSessions is null || activeSessions.Count == 0)
			return;

		foreach (var session in activeSessions)
		{
			session.Expire();

			// Release reserved stock for all items in this session
			var releaseItems = session.Items
				.Select(i => new ReleaseStockItem(i.ProductId, i.Quantity))
				.ToList();

			await sender.Send(new ReleaseStockRequest(releaseItems), cancellationToken);
		}

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);
	}
}
