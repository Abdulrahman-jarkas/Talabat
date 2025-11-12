using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Domain.ProductAggregate;

namespace Talabat.Vender.Application;

public static class ProductDtoToDomainMapper
{
	public static Product ToDomain(this ProductDto dto)
	{
		var modifierGroups = dto.Groups?.Select(g => g.ToDomain()).ToList() ?? new();

		var customization = Customization.Create(modifierGroups);

		return new Product(
			title: dto.Title,
			basePrice: dto.Price,
			customization: customization,
			taxCategoryId: dto.TaxCategoryId
		);
	}

	public static ModifierGroup ToDomain(this ModifierGroupDto dto)
	{
		var items = dto.Items?.Select(d => d.ToDomain()).ToList() ?? new();

		return new ModifierGroup(
			title: dto.Title,
			min: dto.Min,
			max: dto.Max,
			items
		);
	}

	public static ModifierGroupItem ToDomain(this ModifierGroupDto.ModifierGroupItemDto dto)
	{
		var subGroups = dto.SubGroups?.Select(s => s.ToDomain()).ToList() ?? new();

		return new ModifierGroupItem
		{
			ModifierId = dto.ModifierId,
			SubGroups = subGroups
		};
	}

	public static ModifierSubGroup ToDomain(this ModifierSubGroupDto dto)
	{
		return new ModifierSubGroup(dto.ModifierIds)
		{
			Title = dto.Title,
			Min = dto.Min,
			Max = dto.Max
		};
	}
}