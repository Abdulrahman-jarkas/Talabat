using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderPaidEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
