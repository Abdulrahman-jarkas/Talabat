using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Product.Commands.CreateProduct;

internal record CreateProductCommand(
	Guid MerchantId,
	string Title,
	decimal BasePrice,
	int Quantity = 0) : IRequest<ErrorOr<Guid>>;
