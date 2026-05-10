using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.RemoveRole;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Admin;

public class AdminRemoveRoleRequest
{
    public Guid RoleId { get; set; }
    public string Version { get; set; } = null!;
}

public class AdminRemoveRoleValidator : Validator<AdminRemoveRoleRequest>
{
    public AdminRemoveRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }
}

internal class AdminRemoveRoleEndpoint(ISender sender)
    : Endpoint<AdminRemoveRoleRequest>
{
    public override void Configure()
    {
        Delete("/api/admin/roles/{RoleId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.RemoveRole),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminRemoveRoleRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveRoleCommand(req.RoleId, null, req.Version), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
