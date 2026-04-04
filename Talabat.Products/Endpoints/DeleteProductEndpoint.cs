using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.DeleteProduct;
using Talabat.SharedKernal;

namespace Talabat.Products.Endpoints;

public class DeleteProductRequest
{
	public Guid ProductId { get; set; }
}

public class DeleteProductValidator : Validator<DeleteProductRequest>
{
	public DeleteProductValidator()
	{
		RuleFor(x => x.ProductId).NotEmpty();
	}
}

internal class DeleteProductEndpoint(ISender sender)
	: Endpoint<DeleteProductRequest>
{
	public override void Configure()
	{
		Delete("/api/products");
		AllowAnonymous();
	}

	public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
	{
		var result = await sender.Send(new DeleteProductCommand(req.ProductId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
