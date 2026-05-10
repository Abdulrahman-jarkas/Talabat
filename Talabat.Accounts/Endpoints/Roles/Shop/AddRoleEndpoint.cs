using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.AddRole;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles.Shop;

public class AddRoleRequest
{
    public string Name { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
}

public class AddRoleValidator : Validator<AddRoleRequest>
{
    public AddRoleValidator()
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
    }
}

internal class AddRoleEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AddRoleRequest>
{
    public override void Configure()
    {
        Post("/api/shop/roles");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.AddRole),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(AddRoleRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new AddRoleCommand(
            req.Name,
            req.Permissions,
            accountContext.TenantId,
            TenantType.Shop,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
