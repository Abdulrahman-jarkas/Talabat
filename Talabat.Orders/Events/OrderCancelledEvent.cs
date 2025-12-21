using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderCancelledEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
