using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Authorization;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.Admin;

public class AdminGetShopRequest
{
    public Guid ShopId { get; set; }
}

public class AdminGetShopValidator : Validator<AdminGetShopRequest>
{
    public AdminGetShopValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty().WithMessage("ShopId is required.");
    }
}

internal class AdminGetShopEndpoint(ISender sender)
    : Endpoint<AdminGetShopRequest>
{
    public override void Configure()
    {
        Get("/api/admin/shops/{ShopId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminGetShopRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new ShopQuery(req.ShopId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
