using Microsoft.EntityFrameworkCore;
using Talabat.Products.Domain;

namespace Talabat.Products.Data.Repositories;

internal class ProductsRepository(ProductsDbContext context) : IProductsRepository
{
	public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
	}

	public async Task<Product?> GetProductByIdAsync(Guid id, Guid? tenantId, CancellationToken cancellationToken = default)
	{
		var query = context.Products.AsQueryable();
		if (tenantId.HasValue)
			query = query.Where(p => p.ShopId == tenantId.Value);
		return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
	}

	public Task<List<Product>> GetProductsByIdsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default)
	{
		return context.Products
			.Where(p => productIds.Contains(p.Id))
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Product>> GetProductsByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default)
	{
		return await context.Products
			.AsNoTracking()
			.Where(p => p.ShopId == shopId)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
	{
		return await context.Products
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
	{
		await context.Products.AddAsync(product	, cancellationToken);
	}

	public async Task<int> CountByShopAsync(Guid shopId, CancellationToken cancellationToken = default)
	{
		return await context.Products
			.Where(p => p.ShopId == shopId && !p.IsDeleted)
			.CountAsync(cancellationToken);
	}

	public async Task SaveChangesAsync()
	{
		await context.SaveChangesAsync();
	}
}