using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderShippedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
