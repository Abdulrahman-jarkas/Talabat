using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization handler that checks roles from the Accounts module data.
/// </summary>
public sealed class RoleHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IAccountContext _accountContext;

    public RoleHandler(IAccountContext accountContext)
    {
        _accountContext = accountContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // Ensure authorization data is loaded before checking
        var data = await _accountContext.GetAuthorizationDataAsync();
        if (data is null)
            return;

        if (_accountContext.HasRole(requirement.Role))
        {
            context.Succeed(requirement);
        }
    }
}
