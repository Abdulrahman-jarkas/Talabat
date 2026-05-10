using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record ProductQuery(Guid ProductId) : IRequest<ProductResponse?>;

public record ProductsQuery(IReadOnlyList<Guid> ProductIds) : IRequest<List<ProductResponse>?>;