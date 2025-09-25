using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Infrastructure.Persistence.Repositories
{
	public interface IProductsRepository
	{
		Task Add(Product product);
		Task SaveChanges();
	}
}