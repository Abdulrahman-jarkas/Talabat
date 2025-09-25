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

	private static ModifierGroup ToDomain(this GroupDto groupDto)
	{
		// First map all modifiers from options
		var modifiers = new List<Modifier>();
		var modifierIds = new List<Guid>();

		foreach (var option in groupDto.Options)
		{
			var optionModifiers = option.ToDomain();
			modifiers.AddRange(optionModifiers);

			// Collect their IDs
			modifierIds.AddRange(optionModifiers.Select(m => m.Id));
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

	private static List<Modifier> ToDomain(this OptionDto optionDto)
	{
		// Generate a new Modifier
		var modifier = new Modifier(
			optionDto.Title,
			optionDto.Price,
			new List<Guid>() // will be filled by group mapping if needed
		);

		var modifiers = new List<Modifier> { modifier };

		// Handle nested groups recursively
		foreach (var nestedGroup in optionDto.Groups)
		{
			var nestedGroupDomain = nestedGroup.ToDomain();

			// Link modifier to the nested group
			modifier.AddModifierGroup(nestedGroupDomain.Id);
		}

		return modifiers;
	}
}
