using ErrorOr;
using MediatR;
using Talabat.ProductsManagement.Contracts;
using Talabat.Vender.Application;

namespace Talabat.Vender.Integrations;

public class ProductQueryHandler(IProductService productService) : IRequestHandler<ProductDetailsQuery, ProductResponse?>
{
	public async Task<ProductResponse?> Handle(ProductDetailsQuery request, CancellationToken cancellationToken)
	{
		var product = await productService.GetById(request.ProductId);

		if (product is null)
			return null;

		return product!.ToResponse();
	}
}

public static class ProductResponseMapper
{

	public static ProductResponse ToResponse(this ProductDetailsDto productDetailsDto)
	{
		return new ProductResponse(
			productDetailsDto.Id,
			productDetailsDto.Title,
			productDetailsDto.Price,
			productDetailsDto.TaxCategoryId,
			productDetailsDto.Groups.Select(g => g.ToResponse()).ToList());
	}

	public static ModifierGroupResponse ToResponse(this ProductDetailsDto.GroupDetailsDto modifierGroupDto)
	{
		return new ModifierGroupResponse(
			Id: modifierGroupDto.Id,
			Title: modifierGroupDto.Title,
			Min: modifierGroupDto.Min,
			Max: modifierGroupDto.Max,
			Modifiers: modifierGroupDto.Modifiers.Select(i => i.ToResponse()).ToList()
			);
	}

	public static ModifierResponse ToResponse(this ProductDetailsDto.ModifierDetailsDto modifierDto)
	{
		return new ModifierResponse(
			Id: modifierDto.Id,
			Title: modifierDto.Title,
			Price: modifierDto.Price,
			ModifierSubGroups: modifierDto.SubGroups.Select(sg => sg.ToResponse()).ToList()
			);
	}

	public static ModifierSubGroupResponse ToResponse(this ProductDetailsDto.SubGroupDetailsDto modifierDto)
	{
		return new ModifierSubGroupResponse(
			Id: modifierDto.Id,
			Title: modifierDto.Title,
			Min: modifierDto.Min,
			Max: modifierDto.Max,
			Modifiers: modifierDto.Modifiers.Select(m => m.ToResponse()).ToList());
	}

	public static ModifierForSubGroupResponse ToResponse(this ProductDetailsDto.ModifierDetailsForSubGroupDto modifierDto)
	{
		return new ModifierForSubGroupResponse(
			Id: modifierDto.Id,
			Title: modifierDto.Title,
			Price: modifierDto.Price);
	}
}
