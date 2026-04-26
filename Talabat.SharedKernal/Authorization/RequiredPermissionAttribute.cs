namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Attribute to require a specific permission for an endpoint.
/// Used in conjunction with PermissionRequirement and PermissionHandler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequiredPermissionAttribute : Attribute
{
    /// <summary>
    /// The permission required (e.g., "products.create", "orders.read").
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="permission">The required permission.</param>
    public RequiredPermissionAttribute(string permission)
    {
        Permission = permission;
    }
}
