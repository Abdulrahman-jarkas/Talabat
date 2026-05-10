using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Product.Commands.CreateProduct;

internal record CreateProductCommand(
	Guid ShopId,
	string Title,
	decimal BasePrice,
	int Quantity = 0) : IRequest<ErrorOr<Guid>>;
