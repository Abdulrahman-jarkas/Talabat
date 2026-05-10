using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.RoleAggregate.Events;

internal record RoleDeletedEvent(Guid RoleId, Guid? TenantId) : IDomainEvent;
