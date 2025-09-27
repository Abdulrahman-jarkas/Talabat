using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;

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

	public Task AddModifier(Modifier modifier)
	{
		context.Modifiers.Add(modifier);
		return Task.CompletedTask;
	}

	public Task SaveChanges()
	{
		Console.WriteLine(context.ChangeTracker);
		return context.SaveChangesAsync();
	}
}
