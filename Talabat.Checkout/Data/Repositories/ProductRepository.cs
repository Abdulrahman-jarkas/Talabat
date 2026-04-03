using Microsoft.EntityFrameworkCore;
using Talabat.Checkout.Domain.ProductAggregate;

namespace Talabat.Checkout.Data.Repositories;

internal class ProductRepository(CheckoutDbContext context) : IProductRepository
{
	public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
	{
		return context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
	}

	public Task<List<Product>> GetByIdsAsync(IReadOnlyList<Guid> productIds, CancellationToken cancellationToken = default)
	{
		return context.Products
			.Where(p => productIds.Contains(p.Id))
			.ToListAsync(cancellationToken);
	}

	public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
	{
		await context.Products.AddAsync(product, cancellationToken);
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return context.SaveChangesAsync(cancellationToken);
	}
}
