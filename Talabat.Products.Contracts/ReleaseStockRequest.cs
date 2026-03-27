using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record ReleaseStockItem(Guid ProductId, int Quantity);
public record ReleaseStockRequest(List<ReleaseStockItem> Items) : IRequest<ErrorOr<Success>>;
