using FastEndpoints;
using MediatR;
using Talabat.Accounts.Application.Role.Queries.GetRoles;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles;

internal class GetRolesEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/roles");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.ViewRoles));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetRolesQuery(
            accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
