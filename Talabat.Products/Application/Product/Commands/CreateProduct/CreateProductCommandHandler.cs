using ErrorOr;
using MediatR;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Application.Product.Commands.CreateProduct;

internal class CreateProductCommandHandler(IProductsRepository productsRepository)
	: IRequestHandler<CreateProductCommand, ErrorOr<Guid>>
{
	public async Task<ErrorOr<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
	{
		var product = new Domain.Product(
			command.MerchantId,
			command.Title,
			command.BasePrice,
			command.Quantity);

		await productsRepository.AddProductAsync(product, cancellationToken);
		await productsRepository.SaveChangesAsync();

		return product.Id;
	}
}
