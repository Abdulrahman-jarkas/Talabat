using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.UpdateProduct;
using Talabat.SharedKernal;

namespace Talabat.Products.Endpoints;

public class UpdateProductRequest
{
	public Guid ProductId { get; set; }
	public decimal BasePrice { get; set; }
	public int Quantity { get; set; }
}

public class UpdateProductValidator : Validator<UpdateProductRequest>
{
	public UpdateProductValidator()
	{
		RuleFor(x => x.ProductId).NotEmpty();
		RuleFor(x => x.BasePrice).GreaterThan(0);
		RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
	}
}

internal class UpdateProductEndpoint(ISender sender)
	: Endpoint<UpdateProductRequest>
{
	public override void Configure()
	{
		Put("/api/products");
		AllowAnonymous();
	}

	public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new UpdateProductCommand(req.ProductId, req.BasePrice, req.Quantity), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
