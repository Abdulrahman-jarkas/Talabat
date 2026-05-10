using FastEndpoints;
using MediatR;
using Talabat.Accounts.Application.Account.Queries.GetAccounts;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts.Admin;

public class AdminGetAccountsRequest
{
    [QueryParam]
    public string? TenantType { get; set; }

    [QueryParam]
    public Guid? TenantId { get; set; }
}

internal class AdminGetAccountsEndpoint(ISender sender)
    : Endpoint<AdminGetAccountsRequest>
{
    public override void Configure()
    {
        Get("/api/admin/accounts");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.ViewAccounts),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminGetAccountsRequest req, CancellationToken ct)
    {
        TenantType? parsedType = !string.IsNullOrEmpty(req.TenantType)
            ? Enum.Parse<TenantType>(req.TenantType, ignoreCase: true)
            : null;

        var result = await sender.Send(new GetAccountsQuery(parsedType, req.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
