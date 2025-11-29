using ErrorOr;
using MediatR;
using Talabat.Taxes.Common;
using Talabat.Taxes.Contracts;
using Talabat.Vender.Domain.ProductAggregate.Entities;
using Talabat.Vender.Infrastructure.Persistence.Repositories;
using Talabat.Vender.Interfaces;

namespace Talabat.Vender.Application;

public class ProductService(IProductsRepository productsRepository, ISender sender) : IProductService
{
	public async Task<ErrorOr<Success>> Add(ProductDto productDto)
	{
		var product = productDto.ToDomain();

		var taxPolicy = await sender.Send(new GetTaxQuery(product.TaxId));

		if(taxPolicy == null)
		{
			return Error.NotFound(
				code: "TaxPolicy.NotFound",
				description: $"Tax With Id '{product.TaxId}' was not found."
			);
		}

		await productsRepository.Add(product);
		await productsRepository.SaveChanges();

		return Result.Success;
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

		var modifierIds = product.Customization.ModifierGroups
			  .SelectMany(g => g.Items)
			  .SelectMany(i => i.SubGroups.SelectMany(sg => sg.ModifierIds).Prepend(i.ModifierId))
			  .Distinct()
			  .ToList();

		var modifiers = await productsRepository.GetModifiers(modifierIds);


		var details = new ProductDetailsDto()
		{
			Id = product.Id,
			Title = product.Title,
			Price = product.BasePrice
		};

		return product.ToDetailsDto(modifiers.ToDictionary(x => x.Id));
	}

	public async Task<PaginatedResult<ModifierDto>> GetPaginatedModifiersAsync(int pageSize, int? lastId)
	{
		var countTask = productsRepository.GetModifiersEstimatedRowCountAsync();
		var itemsTask = productsRepository.GetPaginatedModifiersAsync(pageSize, lastId);

		await Task.WhenAll(countTask, itemsTask);

		var items = itemsTask.Result.Select(p => p.ToDto()).ToList();
		var totalCount = countTask.Result;

		return new PaginatedResult<ModifierDto>
		{
			Items = items,
			TotalCount = totalCount,
			LastId = lastId
		};
	}

	public async Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(int pageSize, int? lastId)
	{
		var count = await productsRepository.GetProductsEstimatedRowCountAsync();
		var items= await productsRepository.GetPaginatedProductsAsync(pageSize, lastId);

		return new PaginatedResult<ProductDto>
		{
			Items = items.Select(p => p.ToDto()).ToList(),
			TotalCount = count,
			LastId = lastId
		};
	}
}