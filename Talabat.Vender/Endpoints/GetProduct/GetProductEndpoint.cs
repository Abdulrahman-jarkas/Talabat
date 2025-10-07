using FastEndpoints;
using Talabat.Vender.Application;

namespace Talabat.Vender.Endpoints.GetProduct;

public class GetProductEndpoint(IProductService productService) : Endpoint<GetProductRequest, ProductDetailsDto>
{
	public override void Configure()
	{
		Get("/api/products/{Id:int}");
		AllowAnonymous();
	}

	public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
	{
		var product = await productService.GetById(req.Id);

		if(product is null)
		{
			await Send.NotFoundAsync(ct);
			return;
		}

		await Send.OkAsync(product);
	}
}
