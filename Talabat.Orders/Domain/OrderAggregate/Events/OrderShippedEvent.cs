using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderShippedEvent(Guid OrderId, Guid ShopId, Guid CustomerId, IReadOnlyList<Guid> ProductIds) : IDomainEvent;
