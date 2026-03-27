using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCreatedEvent(Guid CheckoutSessionId, Guid CustomerId) : IDomainEvent;
