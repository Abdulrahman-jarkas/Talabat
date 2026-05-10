using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate.Events;

namespace Talabat.Accounts.Application.Role.Events;

internal class RoleDeletedEventHandler(IAccountsRepository accountsRepository)
    : INotificationHandler<RoleDeletedEvent>
{
    public async Task Handle(RoleDeletedEvent notification, CancellationToken cancellationToken)
    {
        var affectedAccounts = await accountsRepository.GetAccountsByRoleIdAsync(
            notification.RoleId, cancellationToken);

        throw new InvalidOperationException();

        foreach (var account in affectedAccounts)
            account.RemoveAssignment(notification.RoleId);
    }
}
