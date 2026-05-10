using Talabat.SharedKernal;

namespace Talabat.Products.Domain.Events;

internal record ProductSoftDeletedEvent(Guid ProductId) : IDomainEvent;
