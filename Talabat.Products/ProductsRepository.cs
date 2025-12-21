using Microsoft.EntityFrameworkCore;

namespace Talabat.Products;

internal class ProductsRepository(ProductsDbContext context) : IProductsRepository
{
	public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Products.FindAsync(id, cancellationToken);
	}

	public Task<List<Product>> GetProductsByIdsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default)
	{
		return context.Products
			.Where(p => productIds.Contains(p.Id))
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<Product>> GetProductsByMerchantIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
	{
		return await context.Products
			.AsNoTracking()
			.Where(p => p.MerchantId == merchantId)
			.ToListAsync(cancellationToken);
	}

	public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
	{
		await context.Products.AddAsync(product	, cancellationToken);
	}

	public async Task SaveChangesAsync()
	{
		await context.SaveChangesAsync();
	}

	
}