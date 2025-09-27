using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Domain.ProductAggregate;
using Talabat.Vender.Dto;
using Talabat.Vender.Endpoints.AddProduct;
using static Talabat.Vender.Endpoints.AddProduct.AddProductRequest;

namespace Talabat.Vender.Mappers;

public static class ProductMapper
{
	public static ProductDto ToDto(this AddProductRequest request)
	{
		return new ProductDto()
		{
			Title = request.Title,
			Price = request.Price,
			Groups = request.Groups.Select(g => g.ToDto()).ToList()
		};
	}

	public static GroupDto ToDto(this Group group)
	{
		return new GroupDto()
		{
			Title = group.Title,
			Min = group.Min,
			Max = group.Max,
			Options = group.Options.Select(option => option.ToDto()).ToList()
		};
	}

	public static OptionDto ToDto(this Option option)
	{
		return new OptionDto()
		{
			Title = option.Title,
			Price = option.Price,
			Groups = option.Groups.Select(g => g.ToDto()).ToList()
		};
	}

	public static Product ToDomain(this ProductDto dto)
	{
		// Map top-level modifier groups
		var modifierGroups = dto.Groups
			.Select(ToDomain)
			.ToList();

		return new Product(
			dto.Title,
			dto.Price,
			modifierGroups
		);
	}

	public static ModifierGroup ToDomain(this GroupDto groupDto)
	{
		var modifierIds = new List<Guid>();

		foreach (var optionDto in groupDto.Options)
		{
			var option = optionDto.ToDomain();
			modifierIds.Add(option.Id);
		}

		// Create the ModifierGroup with collected modifier IDs
		var group = new ModifierGroup(
			groupDto.Title,
			groupDto.Min,
			groupDto.Max,
			modifierIds
		);

		return group;
	}

	public static Modifier ToDomain(this OptionDto optionDto)
	{
		var groupsIds = new List<Guid>();

		foreach(var groupDto in optionDto.Groups)
		{
			var group = groupDto.ToDomain();

			groupsIds.Add(group.Id);
		}

		return new Modifier(
			title: optionDto.Title,
			price: optionDto.Price,
			groupsIds);
	}
}
