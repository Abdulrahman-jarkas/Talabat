using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Application;

public static class ProductMappingExtensions
{
	public static ProductDto ToDto(this Product product)
	{
		if (product == null) return null!;

		return new ProductDto
		{
			Id = product.Id,
			Title = product.Title,
			Price = product.BasePrice,
			Groups = product.Customization.ModifierGroups
				.Select(g => g.ToDto())
				.ToList()
		};
	}

	public static ModifierGroupDto ToDto(this ModifierGroup group)
	{
		return new ModifierGroupDto
		{
			Title = group.Title,
			Min = group.Min,
			Max = group.Max,
			Items = group.Items
				.Select(i => i.ToDto())
				.ToList()
		};
	}

	public static ModifierGroupDto.ModifierGroupItemDto ToDto(this ModifierGroupItem item)
	{
		return new ModifierGroupDto.ModifierGroupItemDto
		{
			ModifierId = item.ModifierId,
			SubGroups = item.SubGroups
				.Select(s => s.ToDto())
				.ToList()
		};
	}

	public static ModifierSubGroupDto ToDto(this ModifierSubGroup subGroup)
	{
		return new ModifierSubGroupDto
		{
			Title = subGroup.Title,
			Min = subGroup.Min,
			Max = subGroup.Max,
			ModifierIds = subGroup.ModifierIds
		};
	}

	public static ModifierDto ToDto(this Modifier modifier)
	{
		return new ModifierDto
		{
			Id = modifier.Id,
			Title = modifier.Title,
			Price = modifier.Price
		};
	}
}
