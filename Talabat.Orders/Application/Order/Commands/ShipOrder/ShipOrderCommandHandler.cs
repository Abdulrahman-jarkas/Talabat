using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Commands.ShipOrder;

internal class ShipOrderCommandHandler(IOrdersRepository ordersRepository)
	: IRequestHandler<ShipOrderCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.GetByIdAsync(command.OrderId, cancellationToken);

		if (order is null)
			return OrderErrors.NotFound;

		// Verify the order belongs to the requesting shop
		if (order.ShopId != command.ShopId)
			return OrderErrors.NotFound;

		var result = order.Ship();
		if (result.IsError)
			return result.Errors;

		using var scope = ModuleTransactionScope.Create();

		await ordersRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		return Result.Success;
	}
}
