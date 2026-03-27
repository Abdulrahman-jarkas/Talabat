using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCancelledEvent(Guid CheckoutSessionId, Guid CustomerId) : IDomainEvent;
