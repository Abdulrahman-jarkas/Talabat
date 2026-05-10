using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Authorization;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.Customer;

public class GetShopRequest
{
    public Guid ShopId { get; set; }
}

public class GetShopValidator : Validator<GetShopRequest>
{
    public GetShopValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty().WithMessage("ShopId is required.");
    }
}

internal class GetShopEndpoint(ISender sender)
    : Endpoint<GetShopRequest>
{
    public override void Configure()
    {
        Get("/api/shops/{ShopId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(GetShopRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new ShopQuery(req.ShopId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
