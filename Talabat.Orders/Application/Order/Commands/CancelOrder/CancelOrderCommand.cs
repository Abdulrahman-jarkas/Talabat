using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.Order.Commands.CancelOrder;

internal record CancelOrderCommand(Guid OrderId) : IRequest<ErrorOr<Success>>;
