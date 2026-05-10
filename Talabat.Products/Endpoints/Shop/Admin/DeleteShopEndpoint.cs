using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Shop.Commands.DeleteShop;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.Admin;

public class AdminDeleteShopRequest
{
    public Guid ShopId { get; set; }
}

public class AdminDeleteShopValidator : Validator<AdminDeleteShopRequest>
{
    public AdminDeleteShopValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty().WithMessage("ShopId is required.");
    }
}

internal class AdminDeleteShopEndpoint(ISender sender)
    : Endpoint<AdminDeleteShopRequest>
{
    public override void Configure()
    {
        Delete("/api/admin/shops/{ShopId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.Delete),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminDeleteShopRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteShopCommand(req.ShopId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
