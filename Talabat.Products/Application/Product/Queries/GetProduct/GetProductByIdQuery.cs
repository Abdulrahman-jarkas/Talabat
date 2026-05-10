using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;

namespace Talabat.Products.Application.Product.Queries.GetProduct;

internal record GetProductByIdQuery(Guid ProductId, Guid? TenantId) : IRequest<ErrorOr<ProductResponse>>;
