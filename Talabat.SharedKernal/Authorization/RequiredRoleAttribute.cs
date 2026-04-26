namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Attribute to require a specific role for an endpoint.
/// Used for role-based authorization (e.g., Customer role for cart operations).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequiredRoleAttribute : Attribute
{
    /// <summary>
    /// The role required (e.g., "Customer", "ShopOwner", "Admin").
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="role">The required role.</param>
    public RequiredRoleAttribute(string role)
    {
        Role = role;
    }
}
