using Talabat.SharedKernal;

namespace Talabat.Products.Domain.Events;

internal record ProductQuantityChangedEvent(Guid ProductId, int NewQuantity) : IDomainEvent;
