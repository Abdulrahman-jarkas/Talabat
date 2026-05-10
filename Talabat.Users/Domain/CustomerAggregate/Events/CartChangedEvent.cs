using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Events;

internal record CartChangedEvent(Guid CustomerId) : IDomainEvent;
