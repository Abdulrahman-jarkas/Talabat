using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Account.Commands.AddAccount;
using Talabat.Accounts.Contracts;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Accounts;

public class AddAccountRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string TenantType { get; set; } = null!;
    public List<Guid> RoleIds { get; set; } = new();
}

public class AddAccountValidator : Validator<AddAccountRequest>
{
    public AddAccountValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.TenantType).NotEmpty();
    }
}

internal class AddAccountEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AddAccountRequest>
{
    public override void Configure()
    {
        Post("/api/accounts");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.AddAccount));
    }

    public override async Task HandleAsync(AddAccountRequest req, CancellationToken ct)
    {
        var tenantType = Enum.Parse<TenantType>(req.TenantType, ignoreCase: true);

        var result = await sender.Send(new AddAccountCommand(
            req.UserId,
            req.Name,
            req.Email,
            accountContext.TenantId,
            tenantType,
            req.RoleIds,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
