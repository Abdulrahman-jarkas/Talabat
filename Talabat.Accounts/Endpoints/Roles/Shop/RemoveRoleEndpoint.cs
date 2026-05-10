using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.RemoveRole;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Shop;

public class RemoveRoleRequest
{
    public Guid RoleId { get; set; }
    public string Version { get; set; } = null!;
}

public class RemoveRoleValidator : Validator<RemoveRoleRequest>
{
    public RemoveRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }
}

internal class RemoveRoleEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<RemoveRoleRequest>
{
    public override void Configure()
    {
        Delete("/api/shop/roles/{RoleId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.RemoveRole),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(RemoveRoleRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveRoleCommand(
            req.RoleId,
            accountContext.TenantId,
            req.Version), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
