using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record ReserveStockItem(Guid ProductId, int Quantity);
public record ReserveStockRequest(List<ReserveStockItem> Items) : IRequest<ErrorOr<Success>>;
