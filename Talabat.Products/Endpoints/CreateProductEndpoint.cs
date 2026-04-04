using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.CreateProduct;
using Talabat.SharedKernal;

namespace Talabat.Products.Endpoints;

public class CreateProductRequest
{
	public Guid MerchantId { get; set; }
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
		RuleFor(x => x.MerchantId).NotEmpty();
		RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
		RuleFor(x => x.BasePrice).GreaterThan(0);
		RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
	}
}

internal class CreateProductEndpoint(ISender sender)
	: Endpoint<CreateProductRequest>
{
	public override void Configure()
	{
		Post("/api/products");
		AllowAnonymous();
	}

	public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new CreateProductCommand(req.MerchantId, req.Title, req.BasePrice, req.Quantity), ct);

		var mapped = result.Then(id => new CreateProductResponse { ProductId = id });

		var (response, statusCode) = mapped.ToCreatedApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
