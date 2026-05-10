using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Queries.GetProduct;
using Talabat.Products.Authorization;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Admin;

public class AdminGetProductRequest
{
    public Guid ProductId { get; set; }
}

public class AdminGetProductValidator : Validator<AdminGetProductRequest>
{
    public AdminGetProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
    }
}

internal class AdminGetProductEndpoint(ISender sender)
    : Endpoint<AdminGetProductRequest>
{
    public override void Configure()
    {
        Get("/api/admin/products/{ProductId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminGetProductRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductByIdQuery(req.ProductId, null), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
