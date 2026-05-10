using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Domain.AccountAggregate.Events;

internal record AccountCreatedEvent(Guid AccountId, Guid UserId, string Email, TenantType TenantType, Guid? TenantId) : IDomainEvent;
