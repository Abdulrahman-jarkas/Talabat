using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.RemoveAccount;
using Talabat.Accounts.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts.Admin;

public class AdminRemoveAccountRequest
{
    public Guid AccountId { get; set; }
    public string Version { get; set; } = null!;
}

public class AdminRemoveAccountValidator : Validator<AdminRemoveAccountRequest>
{
    public AdminRemoveAccountValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId is required.");
    }
}

internal class AdminRemoveAccountEndpoint(ISender sender)
    : Endpoint<AdminRemoveAccountRequest>
{
    public override void Configure()
    {
        Delete("/api/admin/accounts/{AccountId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.RemoveAccount),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminRemoveAccountRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveAccountCommand(req.AccountId, null, req.Version), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
