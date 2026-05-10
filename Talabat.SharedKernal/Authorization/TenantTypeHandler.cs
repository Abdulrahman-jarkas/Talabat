using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization handler that checks the caller's tenant type matches the required type.
/// </summary>
public sealed class TenantTypeHandler : AuthorizationHandler<TenantTypeRequirement>
{
    private readonly IAccountContext _accountContext;

    public TenantTypeHandler(IAccountContext accountContext)
    {
        _accountContext = accountContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantTypeRequirement requirement)
    {
        var data = await _accountContext.GetAuthorizationDataAsync();
        if (data is null)
            return;

        if (string.Equals(_accountContext.TenantType, requirement.TenantType, StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
