using Talabat.SharedKernal;

namespace Talabat.Orders.Events;

public record OrderAcceptedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
