using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderDeliveredEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
