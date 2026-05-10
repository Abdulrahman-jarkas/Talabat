using Talabat.SharedKernal.Authorization;

namespace Talabat.SharedKernal;

public static class TenantValidation
{
    /// <summary>
    /// Resolves the effective tenant ID. If the token has a TenantId and the request also provides one,
    /// they must match. Returns (resolvedId, isValid).
    /// </summary>
    public static (Guid? TenantId, bool IsValid) ResolveTenantId(this IAccountContext accountContext, Guid? requestTenantId)
    {
        if (accountContext.TenantId is null)
            return (requestTenantId, true);

        if (requestTenantId.HasValue && requestTenantId.Value != accountContext.TenantId.Value)
            return (null, false);

        return (accountContext.TenantId, true);
    }
}
