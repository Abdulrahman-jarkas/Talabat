namespace Talabat.SharedKernal.IntegrationEvents;

public record ShopCreatedIntegrationEvent(Guid ShopId) : IIntegrationEvent;
