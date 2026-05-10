using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionClosedEvent(Guid CheckoutSessionId, Guid CustomerId, IReadOnlyList<Guid> ProductIds) : IDomainEvent;
