using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Application;

public static class ProductDomainToDetailsDtoMapper
{
	public static ProductDetailsDto ToDetailsDto(this Product product, Dictionary<int, Modifier> modifiers)
	{
		return new ProductDetailsDto
		{
			Id = product.Id,
			Title = product.Title,
			Price = product.BasePrice,
			Groups = product.Customization.ModifierGroups
				.Select(g => g.ToDetailsDto(modifiers))
				.ToList(),
			TaxCategoryId = product.TaxId,
		};
	}

	public static ProductDetailsDto.GroupDetailsDto ToDetailsDto(
		this ModifierGroup group,
		Dictionary<int, Modifier> modifiers)
	{
		return new ProductDetailsDto.GroupDetailsDto
		{
			Id = group.Id,
			Title = group.Title,
			Min = group.Min,
			Max = group.Max,
			Modifiers = group.Items
				.Select(i => i.ToDetailsDto(modifiers))
				.ToList()
		};
	}

	public static ProductDetailsDto.ModifierDetailsDto ToDetailsDto(
		this ModifierGroupItem item,
		Dictionary<int, Modifier> modifiers)
	{
		var modifier = modifiers.TryGetValue(item.ModifierId, out var m)
			? m
			: null;

		return new ProductDetailsDto.ModifierDetailsDto
		{
			Id = item.ModifierId,
			Title = modifier?.Title ?? $"Modifier #{item.ModifierId}",
			Price = modifier?.Price ?? 0,
			SubGroups = item.SubGroups
				.Select(sg => sg.ToDetailsDto(modifiers))
				.ToList()
		};
	}

	public static ProductDetailsDto.SubGroupDetailsDto ToDetailsDto(
		this ModifierSubGroup subGroup,
		Dictionary<int, Modifier> modifiers)
	{
		return new ProductDetailsDto.SubGroupDetailsDto
		{
			Id = subGroup.Id,
			Title = subGroup.Title,
			Min = subGroup.Min,
			Max = subGroup.Max,
			Modifiers = subGroup.ModifierIds
				.Select(id => id.ToSubGroupModifierDto(modifiers))
				.ToList()
		};
	}

	public static ProductDetailsDto.ModifierDetailsForSubGroupDto ToSubGroupModifierDto(
		this int modifierId,
		Dictionary<int, Modifier> modifiers)
	{
		var modifier = modifiers.TryGetValue(modifierId, out var m)
			? m
			: null;

		return new ProductDetailsDto.ModifierDetailsForSubGroupDto
		{
			Id = modifierId,
			Title = modifier?.Title ?? $"",
			Price = modifier?.Price ?? 0
		};
	}
}
