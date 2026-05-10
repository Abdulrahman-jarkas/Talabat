namespace Talabat.SharedKernal.IntegrationEvents;

public record ProductSoftDeletedIntegrationEvent(Guid ProductId) : IIntegrationEvent;
