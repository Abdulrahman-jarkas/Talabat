using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.EditRole;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Admin;

public class AdminEditRoleRequest
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
    public string Version { get; set; } = null!;
}

public class AdminEditRoleValidator : Validator<AdminEditRoleRequest>
{
    public AdminEditRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions must not be null.")
            .NotEmpty().WithMessage("At least one permission is required.")
            .Must(p => p.Count == p.Distinct().Count())
            .WithMessage("Permissions must not contain duplicates.")
            .ForEach(p => p.NotEmpty().WithMessage("Each permission must be a non-empty string."));
    }
}

internal class AdminEditRoleEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AdminEditRoleRequest>
{
    public override void Configure()
    {
        Put("/api/admin/roles/{RoleId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.EditRole),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminEditRoleRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new EditRoleCommand(
            req.RoleId,
            req.Name,
            req.Permissions,
            null,
            accountContext.AccountId ?? Guid.Empty,
            req.Version), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
