using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Product.Commands.DeleteProduct;

internal record DeleteProductCommand(Guid ProductId, Guid? TenantId = null) : IRequest<ErrorOr<Success>>;
