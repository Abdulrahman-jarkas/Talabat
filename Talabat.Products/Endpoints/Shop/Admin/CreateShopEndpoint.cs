using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Shop.Commands.CreateShop;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.Admin;

public class AdminCreateShopRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AdminCreateShopResponse
{
    public Guid ShopId { get; set; }
}

public class AdminCreateShopValidator : Validator<AdminCreateShopRequest>
{
    public AdminCreateShopValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}

internal class AdminCreateShopEndpoint(ISender sender)
    : Endpoint<AdminCreateShopRequest>
{
    public override void Configure()
    {
        Post("/api/admin/shops");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.Create),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminCreateShopRequest req, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateShopCommand(req.Name, req.Description), ct);

        var mapped = result.Then(id => new AdminCreateShopResponse { ShopId = id });

        var (response, statusCode) = mapped.ToCreatedApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
