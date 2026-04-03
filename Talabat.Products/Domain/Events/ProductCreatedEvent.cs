using Talabat.SharedKernal;

namespace Talabat.Products.Domain.Events;

internal record ProductCreatedEvent(Guid ProductId, decimal BasePrice, int Quantity) : IDomainEvent;
