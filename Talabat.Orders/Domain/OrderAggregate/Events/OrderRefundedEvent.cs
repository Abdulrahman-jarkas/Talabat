using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderRefundedEvent(Guid OrderId, Guid ShopId, Guid CustomerId) : IDomainEvent;
