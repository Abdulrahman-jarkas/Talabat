using MediatR;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.ProductAggregate;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Checkout.Integration;

internal class OnProductCreatedEventHandler(
	IProductRepository productRepository) : INotificationHandler<ProductCreatedIntegrationEvent>
{
	public async Task Handle(ProductCreatedIntegrationEvent notification, CancellationToken cancellationToken)
	{
		var existingProduct = await productRepository.GetByIdAsync(notification.ProductId, cancellationToken);
		if (existingProduct is not null)
			return;

		var product = new Product(notification.ProductId, notification.Quantity, notification.BasePrice);
		await productRepository.AddAsync(product, cancellationToken);
		await productRepository.SaveChangesAsync(cancellationToken);
	}
}
