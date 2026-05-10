using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Validates that the account_version in the JWT matches the current database version.
/// If mismatched, the account data has changed and the token is stale — request is rejected.
/// </summary>
public sealed class AccountVersionHandler : AuthorizationHandler<AccountVersionRequirement>
{
    private readonly IAccountContext _accountContext;

    public AccountVersionHandler(IAccountContext accountContext)
    {
        _accountContext = accountContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AccountVersionRequirement requirement)
    {
        var tokenVersion = _accountContext.GetTokenVersion();
        if (string.IsNullOrEmpty(tokenVersion))
            return; // fail — no version claim

        var data = await _accountContext.GetAuthorizationDataAsync();
        if (data is null)
            return; // fail — account not found/inactive/deleted

        var currentVersion = Convert.ToBase64String(data.Version);

        if (string.Equals(tokenVersion, currentVersion, StringComparison.Ordinal))
        {
            context.Succeed(requirement);
        }
        // else: requirement not succeeded → authorization fails
    }
}
