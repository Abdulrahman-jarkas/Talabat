using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate.Events;

public record CheckoutSessionCompletedEvent(
	Guid CheckoutSessionId,
	Guid CustomerId,
	Guid MerchantId,
	Guid PaymentId,
	Guid AddressId,
	IReadOnlyList<CheckoutCompletedItem> Items) : IDomainEvent;

public record CheckoutCompletedItem(Guid ProductId, int Quantity);
