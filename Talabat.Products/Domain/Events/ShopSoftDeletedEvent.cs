using Talabat.SharedKernal;

namespace Talabat.Products.Domain.Events;

internal record ShopSoftDeletedEvent(Guid ShopId) : IDomainEvent;
