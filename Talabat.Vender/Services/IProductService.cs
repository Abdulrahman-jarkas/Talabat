using Talabat.Vender.Dto;

namespace Talabat.Vender.Services
{
	public interface IProductService
	{
		Task AddProduct(ProductDto productDto);
	}
}