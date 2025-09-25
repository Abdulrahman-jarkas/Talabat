using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Infrastructure.Persistence.Repositories;

public class ProductsRepository(ProductsManagementDbContext context) : IProductsRepository
{
	public Task Add(Product product)
	{
		context.Add(product);
		return Task.CompletedTask;
	}

	public Task SaveChanges()
	{
		return context.SaveChangesAsync();
	}
}
