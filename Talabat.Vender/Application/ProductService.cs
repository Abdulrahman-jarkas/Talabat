using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Interfaces;

namespace Talabat.Vender.Application;

public class ProductService(IProductsRepository productsRepository) : IProductService
{
	public async Task Add(ProductDto productDto)
	{
		var product = new Product(productDto.Title, productDto.Price, []);

		var groups = await productsRepository.GetModifierGroups(productDto.GroupIds);

		foreach (var group in groups)
		{
			product.AddModifierGroup(group);
		}

		await productsRepository.Add(product);
		await productsRepository.SaveChanges();
	}

	public async Task AddModifierGroup(ModifierGroupDto modifierGroupDto)
	{
		var group = new ModifierGroup(
			title: modifierGroupDto.Title,
			min: modifierGroupDto.Min,
			max: modifierGroupDto.Max,
			modifierGroupData: new ModifierGroupData(
				modifierGroupDto
				.Data
				.Select(o => new ModifierGroupItem() { ModifierId = o.ModifierId, GroupIds = o.GroupIds }).ToList())
			);

		await productsRepository.AddModifierGroup(group);
		await productsRepository.SaveChanges();
	}

	public async Task AddModifier(ModifierDto modifierDto)
	{
		var modifier = new Modifier(modifierDto.Title, modifierDto.Price);

		await productsRepository.AddModifier(modifier);
		await productsRepository.SaveChanges();
	}

	public async Task<ProductDto?> GetById(Guid id)
	{
		var product = await productsRepository.GetById(id);

		if (product is null) return null;

		return new ProductDto()
		{
			Id = product.Id,
			Title = product.Title,
			Price = product.BasePrice,
			GroupIds = product.ModifierGroups.Select(m => m.Id).ToList()
		};
	}
}