using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.AddAccount;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts.Admin;

public class AdminAddAccountRequest
{
    public Guid UserId { get; set; }
    public string TenantType { get; set; } = null!;
    public Guid? TenantId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}

public class AdminAddAccountValidator : Validator<AdminAddAccountRequest>
{
    public AdminAddAccountValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.TenantType)
            .NotEmpty().WithMessage("TenantType is required.")
            .Must(t => Enum.TryParse<TenantType>(t, ignoreCase: true, out _))
            .WithMessage("TenantType must be a valid value (System, Shop, Customer).");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required when TenantType is Shop.")
            .When(x => string.Equals(x.TenantType, "Shop", StringComparison.OrdinalIgnoreCase));

        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("RoleIds must not be null.")
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("RoleIds must not contain duplicates.")
            .ForEach(id => id.NotEmpty().WithMessage("Each RoleId must be a valid non-empty GUID."));
    }
}

internal class AdminAddAccountEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AdminAddAccountRequest>
{
    public override void Configure()
    {
        Post("/api/admin/accounts");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.AddAccount),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminAddAccountRequest req, CancellationToken ct)
    {
        var parsedType = Enum.Parse<TenantType>(req.TenantType, ignoreCase: true);

        var result = await sender.Send(new AddAccountCommand(
            req.UserId,
            req.TenantId,
            parsedType,
            req.RoleIds,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
