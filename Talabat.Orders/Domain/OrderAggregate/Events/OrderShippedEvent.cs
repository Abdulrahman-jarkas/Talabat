using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderShippedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
