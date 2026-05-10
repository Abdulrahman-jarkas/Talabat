using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.AddRole;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Admin;

public class AdminAddRoleRequest
{
    public string Name { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
    public string TenantType { get; set; } = null!;
    public Guid? TenantId { get; set; }
}

public class AdminAddRoleValidator : Validator<AdminAddRoleRequest>
{
    public AdminAddRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions must not be null.")
            .NotEmpty().WithMessage("At least one permission is required.")
            .Must(p => p.Count == p.Distinct().Count())
            .WithMessage("Permissions must not contain duplicates.")
            .ForEach(p => p.NotEmpty().WithMessage("Each permission must be a non-empty string."));

        RuleFor(x => x.TenantType)
            .NotEmpty().WithMessage("TenantType is required.")
            .Must(t => Enum.TryParse<TenantType>(t, ignoreCase: true, out _))
            .WithMessage("TenantType must be a valid value (System, Shop, Customer).");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required when TenantType is Shop.")
            .When(x => string.Equals(x.TenantType, "Shop", StringComparison.OrdinalIgnoreCase));
    }
}

internal class AdminAddRoleEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AdminAddRoleRequest>
{
    public override void Configure()
    {
        Post("/api/admin/roles");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.AddRole),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminAddRoleRequest req, CancellationToken ct)
    {
        var parsedType = Enum.Parse<TenantType>(req.TenantType, ignoreCase: true);

        var result = await sender.Send(new AddRoleCommand(
            req.Name,
            req.Permissions,
            req.TenantId,
            parsedType,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
