using FastEndpoints;
using MediatR;
using Talabat.Accounts.Application.Role.Queries.GetRoles;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Admin;

public class AdminGetRolesRequest
{
    [QueryParam]
    public string? TenantType { get; set; }

    [QueryParam]
    public Guid? TenantId { get; set; }
}

internal class AdminGetRolesEndpoint(ISender sender)
    : Endpoint<AdminGetRolesRequest>
{
    public override void Configure()
    {
        Get("/api/admin/roles");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.ViewRoles),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminGetRolesRequest req, CancellationToken ct)
    {
        TenantType? parsedType = !string.IsNullOrEmpty(req.TenantType)
            ? Enum.Parse<TenantType>(req.TenantType, ignoreCase: true)
            : null;

        var result = await sender.Send(new GetRolesQuery(parsedType, req.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
