namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Provides access to the current tenant context extracted from JWT claims.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the user ID from the "sub" claim.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the account ID from the "account_id" claim.
    /// </summary>
    Guid? AccountId { get; }

    /// <summary>
    /// Gets the tenant type from the "tenant_type" claim ("customer" | "shop" | "system").
    /// </summary>
    string? TenantType { get; }

    /// <summary>
    /// Gets the tenant ID from the "tenant_id" claim.
    /// Returns null if the claim is empty string (for customer/system tenant types).
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Gets the account display name from the "account_name" claim.
    /// </summary>
    string? AccountName { get; }

    /// <summary>
    /// Gets the set of roles assigned to the user from all "role" claims.
    /// </summary>
    IReadOnlySet<string> Roles { get; }

    /// <summary>
    /// Gets the set of permissions assigned to the user from all "permission" claims.
    /// </summary>
    IReadOnlySet<string> Permissions { get; }

    /// <summary>
    /// Checks if the user has the specified permission.
    /// </summary>
    /// <param name="permission">The permission to check (e.g., "orders.read", "menu.manage").</param>
    /// <returns>True if the user has the permission; otherwise, false.</returns>
    bool HasPermission(string permission);

    /// <summary>
    /// Checks if the user has the specified role.
    /// </summary>
    /// <param name="role">The role to check (e.g., "ShopOwner", "Customer").</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    bool HasRole(string role);
}
