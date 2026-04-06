using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCheckedOutEvent(
	Guid CheckoutSessionId,
	Guid CustomerId,
	IReadOnlyList<CheckoutCheckedOutItem> Items) : IDomainEvent;

public record CheckoutCheckedOutItem(Guid ProductId, int Quantity);
