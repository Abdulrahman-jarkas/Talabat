using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Product.Commands.UpdateProduct;

internal record UpdateProductCommand(
	Guid ProductId,
	string Title,
	decimal BasePrice,
	int Quantity,
	Guid? TenantId = null) : IRequest<ErrorOr<Success>>;
