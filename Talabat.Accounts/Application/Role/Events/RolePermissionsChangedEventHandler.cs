using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate.Events;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Role.Events;

internal class RolePermissionsChangedEventHandler(IAccountsRepository accountsRepository)
    : INotificationHandler<RolePermissionsChangedEvent>
{
    public async Task Handle(RolePermissionsChangedEvent notification, CancellationToken cancellationToken)
    {
        var affectedAccounts = await accountsRepository.GetAccountsByRoleIdAsync(
            notification.RoleId, cancellationToken);

        foreach (var account in affectedAccounts)
            account.MarkPermissionsChanged();

        using var scope = ModuleTransactionScope.Create();

        await accountsRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();
    }
}
