using MediatR;
using Talabat.Accounts.Data;
using Talabat.SharedKernal.Authorization;
using Talabat.SharedKernal.IntegrationEvents;
using RoleEntity = Talabat.Accounts.Domain.RoleAggregate.Role;

namespace Talabat.Accounts.Application.Shop.IntegrationEvents;

internal class ShopCreatedEventHandler(AccountsDbContext db)
    : INotificationHandler<ShopCreatedIntegrationEvent>
{
    public async Task Handle(ShopCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var permissions = TenantPermissionRegistry.GetAllowedPermissions(TenantType.Shop).ToList();

        var ownerRole = RoleEntity.Create(
            "Owner",
            permissions,
            notification.ShopId,
            TenantType.Shop,
            Guid.Empty,
            isDefault: true);

        if (ownerRole.IsError)
            return;

        db.Roles.Add(ownerRole.Value);
        await db.SaveChangesAsync(cancellationToken);
    }
}
