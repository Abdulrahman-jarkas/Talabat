using Microsoft.EntityFrameworkCore;
using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Interfaces;

namespace Talabat.Vender.Infrastructure.Persistence.Repositories;

public class TableEstimate
{
	public int EstimatedRows { get; set; }
}

public class PaginatedResult<T>
{
	public List<T> Items { get; set; } = new();
	public int? LastId { get; set; }
	public long TotalCount { get; set; } // approximate
}

public class ProductsRepository(ProductsManagementDbContext context) : IProductsRepository
{
	public Task Add(Product product)
	{
		context.Products.Add(product);
		return Task.CompletedTask;
	}

	public Task<List<Modifier>> GetModifiers(IEnumerable<int> ids)
	{
		return context.Modifiers
			.Where(g => ids.Contains(g.Id))
			.AsNoTracking()
			.ToListAsync();
	}

	public Task AddModifier(Modifier modifier)
	{
		context.Modifiers.Add(modifier);
		return Task.CompletedTask;
	}

	public Task<Product?> GetById(int id)
	{
		return context.Products
			.Where(p => p.Id == id)
			.AsNoTracking()
			.FirstOrDefaultAsync();
	}

	public Task<int> GetProductsEstimatedRowCountAsync()
	{
		return context.Products.CountAsync();
	}

	public Task<int> GetModifiersEstimatedRowCountAsync()
	{
		return context.Modifiers.CountAsync();
	}

	public Task SaveChanges()
	{
		return context.SaveChangesAsync();
	}

	public Task<List<Product>> GetPaginatedProductsAsync(int pageSize, int? lastId)
	{
		return context.Products
			.Where(p => !lastId.HasValue || p.Id > lastId)
			.OrderBy(p => p.Id)
			.Take(pageSize)
			.AsNoTracking()
			.ToListAsync();
	}

	public Task<List<Modifier>> GetPaginatedModifiersAsync(int pageSize, int? lastId)
	{
		return context.Modifiers
			.Where(p => !lastId.HasValue || p.Id > lastId)
			.OrderBy(p => p.Id)
			.Take(pageSize)
			.AsNoTracking()
			.ToListAsync();
	}
}
