using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderDeliveredEvent(Guid OrderId, Guid ShopId, Guid CustomerId) : IDomainEvent;
