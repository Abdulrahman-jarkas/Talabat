using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionExpiredEvent(Guid CheckoutSessionId, Guid CustomerId) : IDomainEvent;
