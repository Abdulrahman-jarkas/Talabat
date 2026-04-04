using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Product.Commands.DeleteProduct;

internal record DeleteProductCommand(Guid ProductId) : IRequest<ErrorOr<Success>>;
