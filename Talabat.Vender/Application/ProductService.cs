using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Interfaces;

namespace Talabat.Vender.Application;

public class ProductService(IProductsRepository productsRepository) : IProductService
{
	public async Task Add(ProductDto productDto)
	{
		var product = productDto.ToDomain();

		await productsRepository.Add(product);
		await productsRepository.SaveChanges();
	}

	public async Task AddModifier(ModifierDto modifierDto)
	{
		var modifier = new Modifier(modifierDto.Title, modifierDto.Price);

		await productsRepository.AddModifier(modifier);
		await productsRepository.SaveChanges();
	}

	public async Task<ProductDetailsDto?> GetById(int id)
	{
		var product = await productsRepository.GetById(id);

		if (product is null) return null;

		var modifierIds = product.Customization.ModifierGroups
			  .SelectMany(g => g.Items)
			  .SelectMany(i => i.SubGroups.SelectMany(sg => sg.ModifierIds).Prepend(i.ModifierId))
			  .Distinct()
			  .ToList();

		var modifiers = await productsRepository.GetModifiers(modifierIds);


		var details = new ProductDetailsDto()
		{
			Id = product.Id,
			Title = product.Title,
			Price = product.BasePrice
		};

		return product.ToDetailsDto(modifiers.ToDictionary(x => x.Id));
	}
}