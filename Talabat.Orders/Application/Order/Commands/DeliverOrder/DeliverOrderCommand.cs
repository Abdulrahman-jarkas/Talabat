using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.Order.Commands.DeliverOrder;

internal record DeliverOrderCommand(Guid OrderId) : IRequest<ErrorOr<Success>>;
