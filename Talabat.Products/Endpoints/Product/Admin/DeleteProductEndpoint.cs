using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.DeleteProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Admin;

public class AdminDeleteProductRequest
{
    public Guid ProductId { get; set; }
}

public class AdminDeleteProductValidator : Validator<AdminDeleteProductRequest>
{
    public AdminDeleteProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
    }
}

internal class AdminDeleteProductEndpoint(ISender sender)
    : Endpoint<AdminDeleteProductRequest>
{
    public override void Configure()
    {
        Delete("/api/admin/products/{ProductId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Delete),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminDeleteProductRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteProductCommand(req.ProductId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
