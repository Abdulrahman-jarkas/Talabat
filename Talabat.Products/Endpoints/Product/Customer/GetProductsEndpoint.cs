using FastEndpoints;
using MediatR;
using Talabat.Products.Application.Product.Queries.GetProducts;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Customer;

public class GetProductsRequest
{
    [QueryParam]
    public Guid? ShopId { get; set; }
}

internal class GetProductsEndpoint(ISender sender)
    : Endpoint<GetProductsRequest>
{
    public override void Configure()
    {
        Get("/api/products");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(GetProductsRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductsByShopQuery(req.ShopId), ct);

        await HttpContext.Response.SendAsync(ApiResponse.Ok(result), 200);
    }
}
