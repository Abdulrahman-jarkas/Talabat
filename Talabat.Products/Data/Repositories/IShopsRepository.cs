using Talabat.Products.Domain;

namespace Talabat.Products.Data.Repositories;

internal interface IShopsRepository
{
    Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shop>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Shop shop, CancellationToken cancellationToken = default);
    Task SaveChangesAsync();
}
