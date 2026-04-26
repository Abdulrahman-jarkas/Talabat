using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization requirement for role-based access control.
/// </summary>
public sealed class RoleRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The role required to access the resource.
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Creates a new role requirement.
    /// </summary>
    /// <param name="role">The required role.</param>
    public RoleRequirement(string role)
    {
        Role = role;
    }
}
