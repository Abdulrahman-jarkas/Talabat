using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.AddAccount;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts.Shop;

public class AddAccountRequest
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}

public class AddAccountValidator : Validator<AddAccountRequest>
{
    public AddAccountValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("RoleIds must not be null.")
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("RoleIds must not contain duplicates.")
            .ForEach(id => id.NotEmpty().WithMessage("Each RoleId must be a valid non-empty GUID."));
    }
}

internal class AddAccountEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AddAccountRequest>
{
    public override void Configure()
    {
        Post("/api/shop/accounts");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.AddAccount),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(AddAccountRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new AddAccountCommand(
            req.UserId,
            accountContext.TenantId,
            TenantType.Shop,
            req.RoleIds,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
