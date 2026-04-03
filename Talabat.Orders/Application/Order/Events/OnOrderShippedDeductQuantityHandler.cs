using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderShippedDeductQuantityHandler(
	IOrdersRepository ordersRepository,
	ISender sender,
	ILogger<OnOrderShippedDeductQuantityHandler> logger) : INotificationHandler<OrderShippedEvent>
{
	public async Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.GetByIdAsync(notification.OrderId, cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderShipped.OrderNotFound",
					$"Order {notification.OrderId} not found."));

		var items = order.Items
			.Select(i => new ConfirmShipmentItem(i.ProductId, notification.OrderId))
			.ToList();

		var result = await sender.Send(new ConfirmShipmentRequest(items), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderShipped.ShipmentConfirmationFailed",
					$"Failed to confirm shipment for Order {notification.OrderId}."),
				result.Errors);

		logger.LogInformation(
			"[ConfirmShipment] Stock deducted for Order {OrderId}.",
			notification.OrderId);
	}
}
