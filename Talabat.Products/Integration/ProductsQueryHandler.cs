using MediatR;
using Talabat.Products.Contracts;

namespace Talabat.Products.Integration;

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
				p.MerchantId,
				p.BasePrice))
			.ToList();
	}
}