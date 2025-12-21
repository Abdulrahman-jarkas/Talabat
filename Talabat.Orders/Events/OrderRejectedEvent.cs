using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderRejectedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
