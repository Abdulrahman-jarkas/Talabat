using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Application.Order.Commands.ShipOrder;

internal class ShipOrderCommandHandler(IOrdersRepository ordersRepository)
	: IRequestHandler<ShipOrderCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.GetByIdAsync(command.OrderId, cancellationToken);

		if (order is null)
			return OrderErrors.NotFound;

		var result = order.Ship();
		if (result.IsError)
			return result.Errors;

		await ordersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
