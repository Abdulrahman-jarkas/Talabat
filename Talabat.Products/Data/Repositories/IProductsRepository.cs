using Talabat.Products.Domain;

namespace Talabat.Products.Data.Repositories;

// reposioty 
internal interface IProductsRepository
{
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetProductsByIdsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetProductsByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default);
    Task AddProductAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync();
}
