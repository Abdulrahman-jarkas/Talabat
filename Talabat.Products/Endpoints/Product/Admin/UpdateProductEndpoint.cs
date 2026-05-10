using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.UpdateProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Admin;

public class AdminUpdateProductRequest
{
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
}

public class AdminUpdateProductValidator : Validator<AdminUpdateProductRequest>
{
    public AdminUpdateProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("BasePrice must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must not be negative.");
    }
}

internal class AdminUpdateProductEndpoint(ISender sender)
    : Endpoint<AdminUpdateProductRequest>
{
    public override void Configure()
    {
        Put("/api/admin/products/{ProductId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Update),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminUpdateProductRequest req, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateProductCommand(req.ProductId, req.Title, req.BasePrice, req.Quantity), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
