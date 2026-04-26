using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization handler that checks user claims for required role.
/// </summary>
public sealed class RoleHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        if (context.User.IsInRole(requirement.Role) ||
            context.User.HasClaim(ClaimTypes.Role, requirement.Role))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
