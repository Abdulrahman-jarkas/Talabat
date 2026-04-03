namespace Talabat.SharedKernal.IntegrationEvents;

public record ProductCreatedIntegrationEvent(Guid ProductId, decimal BasePrice, int Quantity) : IIntegrationEvent;
