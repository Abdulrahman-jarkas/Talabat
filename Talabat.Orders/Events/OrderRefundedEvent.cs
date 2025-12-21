using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderRefundedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
