using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;

namespace Talabat.Checkout.Integration;

internal class OnCheckoutSessionCompletedReleaseReservationsHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	IProductRepository productRepository,
	ILogger<OnCheckoutSessionCompletedReleaseReservationsHandler> logger) : INotificationHandler<CheckoutSessionCompletedEvent>
{
	public async Task Handle(CheckoutSessionCompletedEvent notification, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByIdAsync(notification.CheckoutSessionId, cancellationToken);
		if (session is null)
			return;

		//@TODO: how to make sure that product exist in this module
		var productIds = session.Items.Select(i => i.ProductId).ToList();
		var products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

		foreach (var item in session.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null)
			{
				//@TODO: is this right to handle the fauilers in this case
				logger.LogWarning(
					"[SessionCompleted] Checkout Product {ProductId} not found; skipping release for session {SessionId}.",
					item.ProductId,
					session.Id);
				continue;
			}

			var releaseResult = product.Release(item.Quantity);

			if (releaseResult.IsError)
			{
				//@TODO: is this right to handle the fauilers in this case
				logger.LogWarning(
					"[SessionCompleted] Could not release {Quantity} units for Product {ProductId} in session {SessionId}. Errors: {Errors}",
					item.Quantity,
					item.ProductId,
					session.Id,
					string.Join(", ", releaseResult.Errors.Select(e => e.Description)));
			}
		}
	}
}
