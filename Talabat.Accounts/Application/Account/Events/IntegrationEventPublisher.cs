using MediatR;
using Talabat.Accounts.Domain.AccountAggregate.Events;
using Talabat.SharedKernal.IntegrationEvents;

namespace Talabat.Accounts.Application.Account.Events;

internal class IntegrationEventPublisher(IPublisher publisher)
    : INotificationHandler<AccountCreatedEvent>
{
    public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new AccountCreatedIntegrationEvent(
                notification.AccountId,
                notification.Email,
                notification.TenantType,
                notification.TenantId),
            cancellationToken);
    }
}
