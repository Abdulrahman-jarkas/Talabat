using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate.Events;

internal record AccountRemovedEvent(Guid AccountId, Guid UserId) : IDomainEvent;
