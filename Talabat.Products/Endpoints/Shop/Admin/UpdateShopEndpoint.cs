using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Shop.Commands.UpdateShop;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.Admin;

public class AdminUpdateShopRequest
{
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AdminUpdateShopValidator : Validator<AdminUpdateShopRequest>
{
    public AdminUpdateShopValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty().WithMessage("ShopId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}

internal class AdminUpdateShopEndpoint(ISender sender)
    : Endpoint<AdminUpdateShopRequest>
{
    public override void Configure()
    {
        Put("/api/admin/shops/{ShopId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.Update),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminUpdateShopRequest req, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateShopCommand(req.ShopId, req.Name, req.Description), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
