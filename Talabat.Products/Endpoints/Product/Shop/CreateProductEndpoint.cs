using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.CreateProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Product.Shop;

public class CreateProductRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
}

public class CreateProductResponse
{
    public Guid ProductId { get; set; }
}

public class CreateProductValidator : Validator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("BasePrice must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must not be negative.");
    }
}

internal class CreateProductEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<CreateProductRequest>
{
    public override void Configure()
    {
        Post("/api/shop/products");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Create),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var shopId = accountContext.TenantId!.Value;

        var result = await sender.Send(
            new CreateProductCommand(shopId, req.Title, req.BasePrice, req.Quantity), ct);

        var mapped = result.Then(id => new CreateProductResponse { ProductId = id });

        var (response, statusCode) = mapped.ToCreatedApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
