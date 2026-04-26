using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Application.Product.Queries.GetProducts;

internal class ProductsQueryHandler(IProductsRepository productsRepository) : IRequestHandler<ProductsQuery, List<ProductResponse>?>
{
	public async Task<List<ProductResponse>?> Handle(ProductsQuery request, CancellationToken cancellationToken)
	{
		var productsResult = await productsRepository.GetProductsByIdsAsync(request.ProductIds, cancellationToken);

		if (productsResult is null || !productsResult.Any())
			return null;

		return productsResult
			.Select(p => new ProductResponse(
				p.Id,
				p.Title,
				p.ShopId,
				p.BasePrice,
				p.Stock.EffectiveQuantity))
			.ToList();
	}
}
