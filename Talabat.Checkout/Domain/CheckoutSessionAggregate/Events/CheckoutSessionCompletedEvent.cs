using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCompletedEvent(
	Guid CheckoutSessionId,
	Guid CustomerId,
	Guid PaymentId) : IDomainEvent;
