using Microsoft.EntityFrameworkCore;
using Talabat.Products.Domain;

namespace Talabat.Products.Data.Repositories;

internal class ShopsRepository(ProductsDbContext context) : IShopsRepository
{
    public async Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Shops.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Shop>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Shops
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        await context.Shops.AddAsync(shop, cancellationToken);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
