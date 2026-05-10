using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Queries.GetProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Shop;

public class GetProductRequest
{
    public Guid ProductId { get; set; }
}

public class GetProductValidator : Validator<GetProductRequest>
{
    public GetProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
    }
}

internal class GetProductEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<GetProductRequest>
{
    public override void Configure()
    {
        Get("/api/shop/products/{ProductId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductByIdQuery(req.ProductId, accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
