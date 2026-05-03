using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.Order.Commands.ShipOrder;

internal record ShipOrderCommand(Guid ShopId, Guid OrderId) : IRequest<ErrorOr<Success>>;
