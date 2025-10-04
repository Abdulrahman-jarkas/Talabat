
namespace Talabat.Vender.Application
{
	public interface IProductService
	{
		Task Add(ProductDto productDto);
		Task<ProductDto?> GetById(Guid id);
		Task AddModifierGroup(ModifierGroupDto modifierGroupDto);
		Task AddModifier(ModifierDto modifierDto);
	}
}