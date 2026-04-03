using MediatR;
using Talabat.Products.Contracts;

namespace Talabat.Checkout.Application.Services;

internal class ProductService(ISender sender) : IProductService
{
	public async Task<List<ProductDetails>?> GetProductDetailsAsync(
		IReadOnlyList<Guid> productIds,
		CancellationToken cancellationToken = default)
	{
		var products = await sender.Send(new ProductsQuery(productIds), cancellationToken);

		return products?.Select(p => new ProductDetails(
			p.Id,
			p.BasePrice,
			p.Quantity)).ToList();
	}
}
