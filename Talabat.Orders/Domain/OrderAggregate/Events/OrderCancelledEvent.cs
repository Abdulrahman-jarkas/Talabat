using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderCancelledEvent(Guid OrderId, Guid ShopId, Guid CustomerId, Guid PaymentId) : IDomainEvent;
