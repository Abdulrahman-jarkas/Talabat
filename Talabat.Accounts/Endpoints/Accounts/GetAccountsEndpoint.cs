using FastEndpoints;
using MediatR;
using Talabat.Accounts.Application.Account.Queries.GetAccounts;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts;

internal class GetAccountsEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/accounts");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.ViewAccounts));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetAccountsQuery(
            accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
