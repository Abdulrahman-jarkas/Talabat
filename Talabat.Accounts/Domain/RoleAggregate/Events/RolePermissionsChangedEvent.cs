using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.RoleAggregate.Events;

internal record RolePermissionsChangedEvent(Guid RoleId, Guid? TenantId) : IDomainEvent;
