using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCompletedEvent(
	Guid CheckoutSessionId,
	Guid CustomerId,
	Guid PaymentId) : IDomainEvent;
