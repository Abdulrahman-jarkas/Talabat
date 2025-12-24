using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Integration;

internal class ProductQueryHandler(IProductsRepository productsRepository) : IRequestHandler<ProductQuery, ProductResponse?>
{
	public async Task<ProductResponse?> Handle(ProductQuery request, CancellationToken cancellationToken)
	{
		var productResult = await productsRepository.GetProductByIdAsync(request.ProductId, cancellationToken);

		if (productResult is null)
			return null;

		return new ProductResponse(
			productResult.Id,
			productResult.Title,
			productResult.MerchantId,
			productResult.BasePrice);
	}
}