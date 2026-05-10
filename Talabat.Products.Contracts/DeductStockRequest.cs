using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record ConfirmShipmentItem(Guid ProductId, Guid OrderId);
public record ConfirmShipmentRequest(List<ConfirmShipmentItem> Items) : IRequest<ErrorOr<Success>>;
