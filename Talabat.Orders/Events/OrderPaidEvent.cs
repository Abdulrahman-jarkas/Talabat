using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderPaidEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
