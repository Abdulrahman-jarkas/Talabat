using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization requirement that ensures the caller's tenant type matches the expected type.
/// </summary>
public sealed class TenantTypeRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The expected tenant type (e.g., "System", "Shop", "Customer").
    /// </summary>
    public string TenantType { get; }

    public TenantTypeRequirement(string tenantType)
    {
        TenantType = tenantType;
    }
}
