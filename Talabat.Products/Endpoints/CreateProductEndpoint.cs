using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.CreateProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints;

public class CreateProductRequest
{
	public Guid ShopId { get; set; }
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
		RuleFor(x => x.ShopId).NotEmpty();
		RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
		RuleFor(x => x.BasePrice).GreaterThan(0);
		RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
	}
}

[RequiredPermission(ProductsPermissions.Create)]
//[EnforcePlanLimit(Features.Products)]
internal class CreateProductEndpoint(ISender sender)
	: Endpoint<CreateProductRequest>
{
	public override void Configure()
	{
		Post("/api/products");
		Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Create));
	}

	public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new CreateProductCommand(req.ShopId, req.Title, req.BasePrice, req.Quantity), ct);

		var mapped = result.Then(id => new CreateProductResponse { ProductId = id });

		var (response, statusCode) = mapped.ToCreatedApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
