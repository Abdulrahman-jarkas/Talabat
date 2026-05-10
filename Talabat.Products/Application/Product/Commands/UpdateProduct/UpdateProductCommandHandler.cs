using ErrorOr;
using MediatR;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Application.Product.Commands.UpdateProduct;

internal class UpdateProductCommandHandler(IProductsRepository productsRepository)
	: IRequestHandler<UpdateProductCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
	{
		var product = await productsRepository.GetProductByIdAsync(command.ProductId, cancellationToken);

		if (product is null)
			return ProductErrors.NotFound(command.ProductId);

		var priceResult = product.UpdatePrice(command.BasePrice);
		if (priceResult.IsError)
			return priceResult.Errors;

		var quantityResult = product.UpdateQuantity(command.Quantity);
		if (quantityResult.IsError)
			return quantityResult.Errors;

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
