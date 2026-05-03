using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization handler that checks permissions from the Accounts module data.
/// </summary>
public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAccountContext _accountContext;

    public PermissionHandler(IAccountContext accountContext)
    {
        _accountContext = accountContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Ensure authorization data is loaded before checking
        var data = await _accountContext.GetAuthorizationDataAsync();
        if (data is null)
            return;

        if (_accountContext.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
