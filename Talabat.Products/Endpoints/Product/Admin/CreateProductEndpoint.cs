using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.CreateProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Admin;

public class AdminCreateProductRequest
{
    public Guid ShopId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
}

public class AdminCreateProductResponse
{
    public Guid ProductId { get; set; }
}

public class AdminCreateProductValidator : Validator<AdminCreateProductRequest>
{
    public AdminCreateProductValidator()
    {
        RuleFor(x => x.ShopId)
            .NotEmpty().WithMessage("ShopId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("BasePrice must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must not be negative.");
    }
}

internal class AdminCreateProductEndpoint(ISender sender)
    : Endpoint<AdminCreateProductRequest>
{
    public override void Configure()
    {
        Post("/api/admin/products");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Create),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(AdminCreateProductRequest req, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateProductCommand(req.ShopId, req.Title, req.BasePrice, req.Quantity), ct);

        var mapped = result.Then(id => new AdminCreateProductResponse { ProductId = id });

        var (response, statusCode) = mapped.ToCreatedApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
