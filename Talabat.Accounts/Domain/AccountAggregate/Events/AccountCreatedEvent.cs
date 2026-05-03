using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate.Events;

internal record AccountCreatedEvent(Guid AccountId, Guid UserId, Guid? TenantId) : IDomainEvent;
