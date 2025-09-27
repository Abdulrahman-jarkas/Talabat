using Talabat.Vender.Dto;
using Talabat.Vender.Infrastructure.Persistence.Repositories;
using Talabat.Vender.Mappers;

namespace Talabat.Vender.Services;

public class ProductService(IProductsRepository productsRepository) : IProductService
{
	public async Task AddProduct(ProductDto productDto)
	{
		var product = productDto.ToDomain();

		await productsRepository.Add(product);

		foreach (var groupDto in productDto.Groups)
		{
			foreach (var optionDto in groupDto.Options)
			{
				await AddModifier(optionDto);
			}
		}

		await productsRepository.SaveChanges();
	}

	public async Task AddModifierGroup(GroupDto groupDto)
	{
		var group = groupDto.ToDomain();
		await productsRepository.AddModifierGroup(group);

		foreach (var optionDto in groupDto.Options)
		{
			await AddModifier(optionDto);
		}
	}

	public async Task AddModifier(OptionDto optionDto)
	{
		var option = optionDto.ToDomain();
		await productsRepository.AddModifier(option);

		foreach (var groupDto in optionDto.Groups)
		{
			 await AddModifierGroup(groupDto);
		}

	}
}
