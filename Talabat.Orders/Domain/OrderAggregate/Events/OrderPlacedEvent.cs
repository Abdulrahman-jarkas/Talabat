using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderPlacedEvent(
	Guid OrderId,
	Guid CustomerId,
	Guid ShopId,
	Guid CheckoutSessionId,
	IReadOnlyList<Guid> ProductIds) : IDomainEvent;
