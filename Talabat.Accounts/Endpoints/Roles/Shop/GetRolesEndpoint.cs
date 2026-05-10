using FastEndpoints;
using MediatR;
using Talabat.Accounts.Application.Role.Queries.GetRoles;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Shop;

internal class GetRolesEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/shop/roles");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.ViewRoles),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        TenantType? parsedType = !string.IsNullOrEmpty(accountContext.TenantType)
            ? Enum.Parse<TenantType>(accountContext.TenantType, ignoreCase: true)
            : null;

        var result = await sender.Send(
            new GetRolesQuery(parsedType, accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
