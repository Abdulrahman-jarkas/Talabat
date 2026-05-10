using System.Collections.Frozen;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Central registry that maps tenant types to their allowed permissions.
/// Each module registers its permissions at startup via <see cref="Register"/>.
/// </summary>
public static class TenantPermissionRegistry
{
    private static readonly Dictionary<TenantType, HashSet<string>> _permissions = [];
    private static FrozenDictionary<TenantType, FrozenSet<string>>? _frozen;

    /// <summary>
    /// Registers a set of permissions as allowed for a given tenant type.
    /// Must be called during application startup before any validation occurs.
    /// </summary>
    public static void Register(TenantType tenantType, IEnumerable<string> permissions)
    {
        if (_frozen is not null)
            throw new InvalidOperationException("Cannot register permissions after the registry has been frozen.");

        if (!_permissions.TryGetValue(tenantType, out var set))
        {
            set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _permissions[tenantType] = set;
        }

        foreach (var permission in permissions)
            set.Add(permission);
    }

    /// <summary>
    /// Freezes the registry, preventing further registrations. Called after all modules have registered.
    /// </summary>
    public static void Freeze()
    {
        _frozen = _permissions.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet(StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns the set of allowed permissions for the given tenant type.
    /// </summary>
    public static IReadOnlySet<string> GetAllowedPermissions(TenantType tenantType)
    {
        if (_frozen is not null)
            return _frozen.TryGetValue(tenantType, out var frozenSet) ? frozenSet : FrozenSet<string>.Empty;

        return _permissions.TryGetValue(tenantType, out var set)
            ? set
            : FrozenSet<string>.Empty;
    }

    /// <summary>
    /// Validates that all given permissions are allowed for the tenant type.
    /// Returns a list of invalid permissions.
    /// </summary>
    public static IReadOnlyList<string> GetInvalidPermissions(TenantType tenantType, IEnumerable<string> permissions)
    {
        var allowed = GetAllowedPermissions(tenantType);
        return permissions.Where(p => !allowed.Contains(p)).ToList();
    }

    /// <summary>
    /// Resets the registry. Intended for testing only.
    /// </summary>
    public static void Reset()
    {
        _permissions.Clear();
        _frozen = null;
    }
}
