using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate.Events;

public record OrderAcceptedEvent(Guid OrderId, Guid MerchantId, Guid CustomerId) : IDomainEvent;
