using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Accounts.Application.Role.Commands.AddRole;
using Talabat.Accounts.Contracts;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Endpoints.Roles;

public class AddRoleRequest
{
    public string Name { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
    public string TenantType { get; set; } = null!;
}

public class AddRoleValidator : Validator<AddRoleRequest>
{
    public AddRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Permissions).NotEmpty();
        RuleFor(x => x.TenantType).NotEmpty();
    }
}

internal class AddRoleEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AddRoleRequest>
{
    public override void Configure()
    {
        Post("/api/roles");
        Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(AccountsPermissions.AddRole));
    }

    public override async Task HandleAsync(AddRoleRequest req, CancellationToken ct)
    {
        var tenantType = Enum.Parse<TenantType>(req.TenantType, ignoreCase: true);

        var result = await sender.Send(new AddRoleCommand(
            req.Name,
            req.Permissions,
            accountContext.TenantId,
            tenantType,
            accountContext.AccountId ?? Guid.Empty), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
