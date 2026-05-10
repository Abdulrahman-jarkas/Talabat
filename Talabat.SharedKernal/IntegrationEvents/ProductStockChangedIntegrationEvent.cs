namespace Talabat.SharedKernal.IntegrationEvents;

public record ProductQuantityChangedIntegrationEvent(Guid ProductId, int NewQuantity) : IIntegrationEvent;
