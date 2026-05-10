using Talabat.SharedKernal;

namespace Talabat.Products.Domain.Events;

internal record ShopCreatedEvent(Guid ShopId) : IDomainEvent;
