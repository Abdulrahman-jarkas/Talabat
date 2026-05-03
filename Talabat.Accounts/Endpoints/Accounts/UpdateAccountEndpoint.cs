using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.UpdateAccount;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts;

public class UpdateAccountRequest
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = null!;
    public List<Guid> RoleIds { get; set; } = new();
}

public class UpdateAccountValidator : Validator<UpdateAccountRequest>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}

internal class UpdateAccountEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<UpdateAccountRequest>
{
    public override void Configure()
    {
        Put("/api/accounts/{AccountId}");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.UpdateAccount));
    }

    public override async Task HandleAsync(UpdateAccountRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateAccountCommand(
            req.AccountId,
            req.Name,
            req.RoleIds,
            accountContext.AccountId ?? Guid.Empty,
            accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
