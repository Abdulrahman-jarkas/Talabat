namespace Talabat.SharedKernal.IntegrationEvents;

public record CartChangedIntegrationEvent(Guid CustomerId) : IIntegrationEvent;
