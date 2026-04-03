using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCancelledEvent(Guid CheckoutSessionId, Guid CustomerId) : IDomainEvent;
