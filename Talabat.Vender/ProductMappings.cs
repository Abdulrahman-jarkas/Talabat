using Talabat.Vender.Application;
using Talabat.Vender.Endpoints.AddModifierGroup;
using Talabat.Vender.Endpoints.AddProduct;

namespace Talabat.Vender;

//internal static class Mapper
//{
//	public static ProductDto ToDto(this AddProductRequest product)
//	{
//		return new ProductDto()
//		{
//			Price = product.Price,
//			Title = product.Title,
//			GroupIds = product.GroupIds
//		};
//	}

//	public static ModifierGroupDto ToDto(this AddModifierGroupRequest group)
//	{
//		return new ModifierGroupDto
//		{
//			Title = group.Title,
//			Min = group.Min,
//			Max = group.Max,
//			Data = group.Data.Select(item => item.ToDto()).ToList()
//		};
//	}

//	public static ModifierGroupDto.ModifierGroupItemDto ToDto(this AddModifierGroupRequest.ModifierGroupItem item)
//	{
//		return new ModifierGroupDto.ModifierGroupItemDto()
//		{
//			ModifierId = item.ModifierId,
//			GroupIds = item.GroupIds
//		};
//	}

//	public static ModifierDto ToDto(this AddModifierRequest option)
//	{
//		return new ModifierDto
//		{
//			Title = option.Title,
//			Price = option.Price
//		};
//	}
//}

public static class ProductMappings
{
	public static ProductDto ToDto(this AddProductRequest request)
	{
		return new ProductDto
		{
			Title = request.Title,
			Price = request.Price,
			Groups = request.Groups?.Select(g => g.ToDto()).ToList() ?? new()
		};
	}

	public static ModifierGroupDto ToDto(this AddModifierGroupRequest request)
	{
		return new ModifierGroupDto
		{
			Title = request.Title,
			Min = request.Min,
			Max = request.Max,
			Items = request.Items?.Select(d => d.ToDto()).ToList() ?? new()
		};
	}

	public static ModifierGroupDto.ModifierGroupItemDto ToDto(this AddModifierGroupRequest.ModifierGroupItem item)
	{
		return new ModifierGroupDto.ModifierGroupItemDto
		{
			ModifierId = item.ModifierId,
			SubGroups = item.SubGroups?.Select(s => s.ToDto()).ToList() ?? new()
		};
	}

	public static ModifierSubGroupDto ToDto(this AddModifierGroupRequest.AddModifierSubGroupRequest subGroup)
	{
		return new ModifierSubGroupDto
		{
			Title = subGroup.Title,
			Min = subGroup.Min,
			Max = subGroup.Max,
			ModifierIds = subGroup.ModifierIds?.ToList() ?? new()
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
