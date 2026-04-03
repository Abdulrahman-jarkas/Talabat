namespace Talabat.SharedKernal.IntegrationEvents;

public record ProductPriceChangedIntegrationEvent(Guid ProductId, decimal NewBasePrice) : IIntegrationEvent;
