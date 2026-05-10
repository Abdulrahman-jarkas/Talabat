using FastEndpoints;
using MediatR;
using Talabat.Products.Application.Product.Queries.GetProducts;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Shop;

internal class GetProductsEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/shop/products");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetProductsByShopQuery(accountContext.TenantId), ct);

        await HttpContext.Response.SendAsync(ApiResponse.Ok(result), 200);
    }
}
