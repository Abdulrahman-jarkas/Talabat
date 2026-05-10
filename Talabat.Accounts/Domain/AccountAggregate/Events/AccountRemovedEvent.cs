using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate.Events;

internal record AccountRemovedEvent(Guid AccountId, IReadOnlyCollection<Guid> PreviousRoleIds) : IDomainEvent;
