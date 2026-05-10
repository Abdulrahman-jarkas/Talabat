using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.DeleteProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Shop;

public class DeleteProductRequest
{
    public Guid ProductId { get; set; }
}

public class DeleteProductValidator : Validator<DeleteProductRequest>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
    }
}

internal class DeleteProductEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<DeleteProductRequest>
{
    public override void Configure()
    {
        Delete("/api/shop/products/{ProductId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Delete),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        var result = await sender.Send(
            new DeleteProductCommand(req.ProductId, accountContext.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
