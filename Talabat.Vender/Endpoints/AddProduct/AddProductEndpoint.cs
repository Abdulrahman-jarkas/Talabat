using FastEndpoints;
using Talabat.Vender.Application;
using Talabat.Vender.Endpoints.GetProduct;

namespace Talabat.Vender.Endpoints.AddProduct;

public class AddProductEndpoint(IProductService productService) : Endpoint<AddProductRequest, ProductDto>
{
	public override void Configure()
	{
		Post("/api/products");
		AllowAnonymous();
	}

	public override async Task HandleAsync(AddProductRequest req, CancellationToken ct)
	{
		var product = req.ToDto();

		await productService.Add(product);

		await Send.CreatedAtAsync(nameof(GetProductEndpoint), new { product.Id });
	}
}