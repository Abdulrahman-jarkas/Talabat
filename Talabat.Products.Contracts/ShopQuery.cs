using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record ShopQuery(Guid ShopId) : IRequest<ErrorOr<ShopResponse>>;

public record ShopsQuery() : IRequest<IReadOnlyList<ShopResponse>>;
