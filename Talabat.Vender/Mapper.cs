using Talabat.Vender.Application;
using Talabat.Vender.Endpoints.AddModifierGroup;
using Talabat.Vender.Endpoints.AddProduct;

namespace Talabat.Vender;

internal static class Mapper
{
	public static ProductDto ToDto(this AddProductRequest product)
	{
		return new ProductDto()
		{
			Price = product.Price,
			Title = product.Title,
			GroupIds = product.GroupIds
		};
	}

	public static ModifierGroupDto ToDto(this AddModifierGroupRequest group)
	{
		return new ModifierGroupDto
		{
			Title = group.Title,
			Min = group.Min,
			Max = group.Max,
			Data = group.Data.Select(item => item.ToDto()).ToList()
		};
	}

	public static ModifierGroupDto.ModifierGroupItemDto ToDto(this AddModifierGroupRequest.ModifierGroupItem item)
	{
		return new ModifierGroupDto.ModifierGroupItemDto()
		{
			ModifierId = item.ModifierId,
			GroupIds = item.GroupIds
		};
	}

	public static ModifierDto ToDto(this AddModifierRequest option)
	{
		return new ModifierDto
		{
			Title = option.Title,
			Price = option.Price
		};
	}
}