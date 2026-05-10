using Talabat.SharedKernal.Authorization;

namespace Talabat.SharedKernal.IntegrationEvents;

public record AccountCreatedIntegrationEvent(Guid AccountId, string Email, TenantType TenantType, Guid? TenantId) : IIntegrationEvent;
