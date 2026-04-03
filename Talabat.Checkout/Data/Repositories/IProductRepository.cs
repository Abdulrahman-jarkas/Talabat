using Talabat.Checkout.Domain.ProductAggregate;

namespace Talabat.Checkout.Data.Repositories;

internal interface IProductRepository
{
	Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
	Task<List<Product>> GetByIdsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default);
	Task AddAsync(Product product, CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
