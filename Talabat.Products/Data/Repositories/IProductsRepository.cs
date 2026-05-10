using Talabat.Products.Domain;

namespace Talabat.Products.Data.Repositories;

internal interface IProductsRepository
{
    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(Guid id, Guid? tenantId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetProductsByIdsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetProductsByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task AddProductAsync(Product product, CancellationToken cancellationToken = default);
    Task<int> CountByShopAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync();
}
