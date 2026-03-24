using Talabat.Products.Contracts;

namespace Talabat.Users.Application.Services;

internal interface IProductService
{
	Task<List<ProductResponse>?> GetProductsDetailsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default);
}
