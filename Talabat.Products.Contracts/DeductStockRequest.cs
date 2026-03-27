using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record DeductStockItem(Guid ProductId, int Quantity);
public record DeductStockRequest(List<DeductStockItem> Items) : IRequest<ErrorOr<Success>>;
