
using Talabat.Vender.Infrastructure.Persistence.Repositories;

namespace Talabat.Vender.Application
{
	public interface IProductService
	{
		Task Add(ProductDto productDto);
		Task AddModifier(ModifierDto modifierDto);
		Task<ProductDetailsDto?> GetById(int id);
		Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(int pageSize, int? lastId);
		Task<PaginatedResult<ModifierDto>> GetPaginatedModifiersAsync(int pageSize, int? lastId);
	}
}