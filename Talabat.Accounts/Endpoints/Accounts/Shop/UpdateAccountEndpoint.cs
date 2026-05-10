using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.UpdateAccount;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts.Shop;

public class UpdateAccountRequest
{
    public Guid AccountId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
    public string Version { get; set; } = null!;
}

public class UpdateAccountValidator : Validator<UpdateAccountRequest>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId is required.");

        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("RoleIds must not be null.")
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("RoleIds must not contain duplicates.")
            .ForEach(id => id.NotEmpty().WithMessage("Each RoleId must be a valid non-empty GUID."));
    }
}

internal class UpdateAccountEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<UpdateAccountRequest>
{
    public override void Configure()
    {
        Put("/api/shop/accounts/{AccountId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.UpdateAccount),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(UpdateAccountRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAccountCommand(
            req.AccountId,
            req.RoleIds,
            accountContext.AccountId ?? Guid.Empty,
            accountContext.TenantId,
            req.Version), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
