using FastEndpoints;
using MediatR;
using Talabat.Products.Application.Product.Queries.GetProducts;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Admin;

public class AdminGetProductsRequest
{
    [QueryParam]
    public Guid? ShopId { get; set; }
}

internal class AdminGetProductsEndpoint(ISender sender)
    : Endpoint<AdminGetProductsRequest>
{
    public override void Configure()
    {
        Get("/api/admin/products");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminGetProductsRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductsByShopQuery(req.ShopId), ct);

        await HttpContext.Response.SendAsync(ApiResponse.Ok(result), 200);
    }
}
