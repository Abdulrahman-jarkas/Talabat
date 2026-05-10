using MediatR;
using Talabat.SharedKernal.IntegrationEvents;
using Talabat.Users.Domain.CustomerAggregate.Events;

namespace Talabat.Users.Application.Customer.Events;

internal class IntegrationEventPublisher(IPublisher publisher)
    : INotificationHandler<CartChangedEvent>
{
    public async Task Handle(CartChangedEvent notification, CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new CartChangedIntegrationEvent(notification.CustomerId),
            cancellationToken);
    }
}
