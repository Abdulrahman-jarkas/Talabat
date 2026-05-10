using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Shop.Commands.UpdateShop;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.ShopOwner;

public class UpdateShopRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateShopValidator : Validator<UpdateShopRequest>
{
    public UpdateShopValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}

internal class UpdateShopEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<UpdateShopRequest>
{
    public override void Configure()
    {
        Put("/api/shop");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.Update),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(UpdateShopRequest req, CancellationToken ct)
    {
        var shopId = accountContext.TenantId!.Value;

        var result = await sender.Send(
            new UpdateShopCommand(shopId, req.Name, req.Description), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
