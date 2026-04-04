using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Product.Commands.UpdateProduct;

internal record UpdateProductCommand(
	Guid ProductId,
	decimal BasePrice,
	int Quantity) : IRequest<ErrorOr<Success>>;
