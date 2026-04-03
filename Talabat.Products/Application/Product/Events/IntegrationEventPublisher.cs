using MediatR;
using Talabat.Products.Domain.Events;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Products.Application.Product.Events;

internal class IntegrationEventPublisher(IPublisher publisher)
	: INotificationHandler<ProductCreatedEvent>,
	  INotificationHandler<ProductPriceChangedEvent>,
	  INotificationHandler<ProductQuantityChangedEvent>,
	  INotificationHandler<ProductSoftDeletedEvent>
{
	public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
	{
		await publisher.Publish(
			new ProductCreatedIntegrationEvent(notification.ProductId, notification.BasePrice, notification.Quantity),
			cancellationToken);
	}

	public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
	{
		await publisher.Publish(
			new ProductPriceChangedIntegrationEvent(notification.ProductId, notification.NewBasePrice),
			cancellationToken);
	}

	public async Task Handle(ProductQuantityChangedEvent notification, CancellationToken cancellationToken)
	{
		await publisher.Publish(
			new ProductQuantityChangedIntegrationEvent(notification.ProductId, notification.NewQuantity),
			cancellationToken);
	}

	public async Task Handle(ProductSoftDeletedEvent notification, CancellationToken cancellationToken)
	{
		await publisher.Publish(
			new ProductSoftDeletedIntegrationEvent(notification.ProductId),
			cancellationToken);
	}
}
