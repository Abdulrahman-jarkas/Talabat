using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.Order.Commands.CancelOrder;

internal record CancelOrderCommand(Guid OrderId, Guid? ShopId = null, Guid? CustomerId = null) : IRequest<ErrorOr<Success>>;
