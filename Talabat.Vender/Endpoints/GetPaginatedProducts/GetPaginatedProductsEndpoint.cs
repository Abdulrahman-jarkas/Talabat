using FastEndpoints;
using Talabat.Vender.Application;
using Talabat.Vender.Infrastructure.Persistence.Repositories;

namespace Talabat.Vender.Endpoints.GetPaginatedProducts;

public class GetPaginatedProductsEndpoint(IProductService productService) : Endpoint<GetPaginatedResultRequest, PaginatedResult<ProductDto>>
{
	public override void Configure()
	{
		Get("/api/products");
		AllowAnonymous();
	}

	public override async Task HandleAsync(GetPaginatedResultRequest req, CancellationToken ct)
	{
		var products = await productService.GetPaginatedProductsAsync(req.PageSize, req.LastId);

		await Send.OkAsync(products);
	}
}
