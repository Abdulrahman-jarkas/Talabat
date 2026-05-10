using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate.Events;

internal record AccountAssignmentsChangedEvent(
    Guid AccountId,
    IReadOnlyCollection<Guid> PreviousRoleIds,
    IReadOnlyCollection<Guid> CurrentRoleIds) : IDomainEvent;
