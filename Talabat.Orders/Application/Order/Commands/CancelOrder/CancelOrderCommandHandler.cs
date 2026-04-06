using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Commands.CancelOrder;

internal class CancelOrderCommandHandler(IOrdersRepository ordersRepository)
	: IRequestHandler<CancelOrderCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.GetByIdAsync(command.OrderId, cancellationToken);

		if (order is null)
			return OrderErrors.NotFound;

		var result = order.Cancel();
		if (result.IsError)
			return result.Errors;

		using var scope = ModuleTransactionScope.Create();

		await ordersRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		return Result.Success;
	}
}
