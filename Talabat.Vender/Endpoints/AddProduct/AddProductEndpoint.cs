using FastEndpoints;
using Talabat.Vender.Mappers;
using Talabat.Vender.Services;

namespace Talabat.Vender.Endpoints.AddProduct;

public class AddProductEndpoint(IProductService productService) : Endpoint<AddProductRequest>
{
	public override void Configure()
	{
		Post("/api/products");
		AllowAnonymous();
	}

	public override async Task HandleAsync(AddProductRequest req, CancellationToken ct)
	{
		var product = req.ToDto();

		await productService.AddProduct(product);
	}
}
