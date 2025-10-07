using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Interfaces;

public interface IProductsRepository
{
	Task Add(Product product);
	Task<Product?> GetById(int id);
	Task AddModifier(Modifier modifier);
	Task<List<Modifier>> GetModifiers(IEnumerable<int> ids);
	Task<List<Product>> GetPaginatedProductsAsync(int pageSize, int? lastId);
	Task<List<Modifier>> GetPaginatedModifiersAsync(int pageSize, int? lastId);
	Task<int> GetProductsEstimatedRowCountAsync();
	Task<int> GetModifiersEstimatedRowCountAsync();
	Task SaveChanges();
}