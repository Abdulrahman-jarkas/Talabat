using MediatR;
using Talabat.SharedKernal.Authorization;
using Talabat.SharedKernal.IntegrationEvents;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Application.Customer.IntegrationEvents;

internal class AccountCreatedEventHandler(IUsersRepository usersRepository)
    : INotificationHandler<AccountCreatedIntegrationEvent>
{
    public async Task Handle(AccountCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (notification.TenantType != TenantType.Customer)
            return;

        var existing = await usersRepository.GetCustomerByIdAsync(notification.AccountId, cancellationToken);
        if (existing is not null)
            return;

        var customer = new Domain.CustomerAggregate.Customer(notification.AccountId, notification.Email);

        await usersRepository.AddAsync(customer, cancellationToken);
        await usersRepository.SaveChangesAsync(cancellationToken);
    }
}
