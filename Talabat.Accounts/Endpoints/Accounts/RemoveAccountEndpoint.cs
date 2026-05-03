using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.RemoveAccount;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts;

public class RemoveAccountRequest
{
    public Guid AccountId { get; set; }
}

public class RemoveAccountValidator : Validator<RemoveAccountRequest>
{
    public RemoveAccountValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
    }
}

internal class RemoveAccountEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<RemoveAccountRequest>
{
    public override void Configure()
    {
        Delete("/api/accounts/{AccountId}");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.RemoveAccount));
    }

    public override async Task HandleAsync(RemoveAccountRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveAccountCommand(
            req.AccountId,
            accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
