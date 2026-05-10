using Talabat.SharedKernal;

namespace Talabat.Products.Domain.Events;

internal record ProductPriceChangedEvent(Guid ProductId, decimal NewBasePrice) : IDomainEvent;
