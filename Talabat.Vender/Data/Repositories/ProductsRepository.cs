using Microsoft.EntityFrameworkCore;
using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Interfaces;

namespace Talabat.Vender.Infrastructure.Persistence.Repositories;

public class ProductsRepository(ProductsManagementDbContext context) : IProductsRepository
{
	public Task Add(Product product)
	{
		context.Products.Add(product);
		return Task.CompletedTask;
	}

	public Task AddModifierGroup(ModifierGroup modifierGroup)
	{
		context.ModifierGroups.Add(modifierGroup);
		return Task.CompletedTask;
	}

	public Task<List<ModifierGroup>> GetModifierGroups(IEnumerable<int> ids)
	{
		return context.ModifierGroups
			.Where(g => ids.Contains(g.Id))
			.AsNoTracking()
			.ToListAsync();
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
			.Include(p => p.ModifierGroups)
			.AsSingleQuery()
			.AsNoTracking()
			.FirstOrDefaultAsync();
	}

	public Task SaveChanges()
	{
		return context.SaveChangesAsync();
	}


}
