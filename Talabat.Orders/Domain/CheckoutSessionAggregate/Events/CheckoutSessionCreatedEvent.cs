using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCreatedEvent(Guid CheckoutSessionId, Guid CustomerId) : IDomainEvent;
