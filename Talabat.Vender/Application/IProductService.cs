
namespace Talabat.Vender.Application
{
	public interface IProductService
	{
		Task Add(ProductDto productDto);
		Task AddModifierGroup(ModifierGroupDto modifierGroupDto);
		Task AddModifier(ModifierDto modifierDto);
		Task<ProductDetailsDto?> GetById(int id);

	}
}