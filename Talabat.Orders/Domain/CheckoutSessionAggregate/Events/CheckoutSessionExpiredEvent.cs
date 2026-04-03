using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionExpiredEvent(Guid CheckoutSessionId, Guid CustomerId) : IDomainEvent;
