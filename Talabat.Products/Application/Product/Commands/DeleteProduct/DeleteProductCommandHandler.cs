using ErrorOr;
using MediatR;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Application.Product.Commands.DeleteProduct;

internal class DeleteProductCommandHandler(IProductsRepository productsRepository)
	: IRequestHandler<DeleteProductCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
	{
		var product = await productsRepository.GetProductByIdAsync(command.ProductId, cancellationToken);

		if (product is null)
			return ProductErrors.NotFound(command.ProductId);

		var result = product.SoftDelete();
		if (result.IsError)
			return result.Errors;

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
