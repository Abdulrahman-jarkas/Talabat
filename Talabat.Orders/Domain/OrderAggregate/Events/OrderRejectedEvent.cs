using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderRejectedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
