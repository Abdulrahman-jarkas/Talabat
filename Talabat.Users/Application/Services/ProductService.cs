using MediatR;
using Talabat.Products.Contracts;

namespace Talabat.Users.Application.Services;

internal class ProductService(ISender sender) : IProductService
{
	public Task<List<ProductResponse>?> GetProductsDetailsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default)
	{
		return sender.Send(new ProductsQuery(productIds), cancellationToken);
	}
}
