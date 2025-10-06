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

	public async Task<ProductDetailsDto?> GetById(int id)
	{
		var product = await productsRepository.GetById(id);

		if (product is null) return null;

		var details = new ProductDetailsDto()
		{
			Id = product.Id,
			Title = product.Title,
			Price = product.BasePrice
		};

		var groupIds = product.ModifierGroups
			.Select(mg => mg.Data.ModifierGroupItems.Select(i => i.GroupIds).SelectMany(x => x))
			.SelectMany(x => x)
			.Distinct();

		var allGroups = new List<ModifierGroup>(product.ModifierGroups);
		var nestedGroups = await GetNestedGroupsAsListAsync(groupIds.ToList());
		allGroups.AddRange(nestedGroups);

		allGroups = allGroups.DistinctBy(x => x.Id).ToList();

		var modifierIds = allGroups
			.Select(g => g.Data.ModifierGroupItems.Select(i => i.ModifierId))
			.SelectMany(x => x)
			.Distinct();

		var modifiers = await productsRepository.GetModifiers(modifierIds.ToList());

		var groups = LinkGroupsAndModifiers(product.ModifierGroups, allGroups, modifiers);

		details.Groups = groups;

		return details;
	}

	private List<ProductDetailsDto.GroupDetailsDto> LinkGroupsAndModifiers(
		List<ModifierGroup> groups,
		List<ModifierGroup> allGroups,
		List<Modifier> allModifiers)
	{
		var list = new List<ProductDetailsDto.GroupDetailsDto>();

		foreach (var group in groups)
		{
			var groupDto = new ProductDetailsDto.GroupDetailsDto()
			{
				Id = group.Id,
				Title = group.Title,
				Min = group.Min,
				Max = group.Max
			};

			foreach (var modifierGroupItem in group.Data.ModifierGroupItems)
			{
				var modifier = allModifiers.FirstOrDefault(m => m.Id == modifierGroupItem.ModifierId);

				if (modifier is not null)
				{

					var modifierDto = new ProductDetailsDto.ModifierDetailsDto()
					{
						Id = modifier.Id,
						Title = modifier.Title,
						Price = modifier.Price
					};

					if (modifierGroupItem.GroupIds.Count > 0)
					{
						var modifierGroups = allGroups.Where(g => modifierGroupItem.GroupIds.Contains(g.Id)).ToList();
						modifierDto.Groups = LinkGroupsAndModifiers(modifierGroups, allGroups, allModifiers);
					}

					groupDto.Modifiers.Add(modifierDto);
				}

			}

			list.Add(groupDto);
		}

		return list;
	}

	private async Task<List<ModifierGroup>> GetNestedGroupsAsListAsync(List<int> groupIds)
	{
		var groups = await productsRepository.GetModifierGroups(groupIds);

		var nestedGroupIds = groups
			.Select(g => g.Data.ModifierGroupItems.Select(i => i.GroupIds).SelectMany(x => x))
			.SelectMany(x => x)
			.ToList();

		if (nestedGroupIds.Count > 0)
		{
			var nestedGroups = await GetNestedGroupsAsListAsync(nestedGroupIds.ToList());
			groups.AddRange(nestedGroups);
		}


		return groups;
	}
}