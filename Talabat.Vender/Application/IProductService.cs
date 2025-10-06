
namespace Talabat.Vender.Application
{
	public interface IProductService
	{
		Task Add(ProductDto productDto);
		Task AddModifier(ModifierDto modifierDto);
		Task<ProductDetailsDto?> GetById(int id);
	}
}