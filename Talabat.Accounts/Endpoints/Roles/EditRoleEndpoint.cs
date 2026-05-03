using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.EditRole;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles;

public class EditRoleRequest
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
}

public class EditRoleValidator : Validator<EditRoleRequest>
{
    public EditRoleValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Permissions).NotEmpty();
    }
}

internal class EditRoleEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<EditRoleRequest>
{
    public override void Configure()
    {
        Put("/api/roles/{RoleId}");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.EditRole));
    }

    public override async Task HandleAsync(EditRoleRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new EditRoleCommand(
            req.RoleId,
            req.Name,
            req.Permissions,
            accountContext.TenantId,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
