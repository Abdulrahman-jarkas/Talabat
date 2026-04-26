using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Provides access to tenant context extracted from the current user's JWT claims.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    private readonly ClaimsPrincipal? _user;
    private readonly Lazy<IReadOnlySet<string>> _roles;
    private readonly Lazy<IReadOnlySet<string>> _permissions;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _user = httpContextAccessor.HttpContext?.User;
        _roles = new Lazy<IReadOnlySet<string>>(() => GetClaimValuesAsSet(AuthorizationClaimTypes.Role));
        _permissions = new Lazy<IReadOnlySet<string>>(() => GetClaimValuesAsSet(AuthorizationClaimTypes.Permission));
    }

    /// <inheritdoc />
    public string? UserId => _user?.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <inheritdoc />
    public Guid? AccountId => ParseGuidClaim(AuthorizationClaimTypes.AccountId);

    /// <inheritdoc />
    public string? TenantType => _user?.FindFirstValue(AuthorizationClaimTypes.TenantType);

    /// <inheritdoc />
    public Guid? TenantId => ParseTenantIdClaim();

    /// <inheritdoc />
    public string? AccountName => _user?.FindFirstValue(AuthorizationClaimTypes.AccountName);

    /// <inheritdoc />
    public IReadOnlySet<string> Roles => _roles.Value;

    /// <inheritdoc />
    public IReadOnlySet<string> Permissions => _permissions.Value;

    /// <inheritdoc />
    public bool HasPermission(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        return Permissions.Contains(permission);
    }

    /// <inheritdoc />
    public bool HasRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        return Roles.Contains(role);
    }

    private Guid? ParseGuidClaim(string claimType)
    {
        var value = _user?.FindFirstValue(claimType);
        if (string.IsNullOrEmpty(value))
            return null;

        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    /// <summary>
    /// Parses the tenant_id claim. Returns null if empty string (for customer/system tenant types).
    /// </summary>
    private Guid? ParseTenantIdClaim()
    {
        var value = _user?.FindFirstValue(AuthorizationClaimTypes.TenantId);

        // tenant_id is empty string for customer/system tenant types
        if (string.IsNullOrEmpty(value))
            return null;

        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    private IReadOnlySet<string> GetClaimValuesAsSet(string claimType)
    {
        if (_user is null)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var values = _user.Claims
            .Where(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return values;
    }
}
