using Talabat.Vender.Dto;
using Talabat.Vender.Infrastructure.Persistence.Repositories;
using Talabat.Vender.Mappers;

namespace Talabat.Vender.Services;

public class ProductService(IProductsRepository productsRepository) : IProductService
{
	public async Task AddProduct(ProductDto productDto)
	{
		var product = productDto.ToDomain();

		await productsRepository.Add(product);
		await productsRepository.SaveChanges();
	}
}
