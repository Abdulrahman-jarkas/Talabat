using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Products.Application.Product.Commands.DeleteProduct;
using Talabat.Products.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

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

[RequiredPermission(ProductsPermissions.Delete)]
internal class DeleteProductEndpoint(ISender sender)
	: Endpoint<DeleteProductRequest>
{
	public override void Configure()
	{
		Delete("/api/products");
		Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(ProductsPermissions.Delete));
	}

	public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
	{
		var result = await sender.Send(new DeleteProductCommand(req.ProductId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
