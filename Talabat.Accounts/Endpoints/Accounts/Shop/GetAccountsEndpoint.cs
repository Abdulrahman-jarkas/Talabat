using FastEndpoints;
using MediatR;
using Talabat.Accounts.Application.Account.Queries.GetAccounts;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts.Shop;

internal class GetAccountsEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/shop/accounts");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.ViewAccounts),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        TenantType? parsedType = !string.IsNullOrEmpty(accountContext.TenantType)
            ? Enum.Parse<TenantType>(accountContext.TenantType, ignoreCase: true)
            : null;

        var result = await sender.Send(
            new GetAccountsQuery(parsedType, accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
