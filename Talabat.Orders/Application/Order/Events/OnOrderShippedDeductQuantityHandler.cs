using MediatR;
using Microsoft.Extensions.Logging;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderShippedDeductQuantityHandler(
	ISender sender,
	ILogger<OnOrderShippedDeductQuantityHandler> logger) : INotificationHandler<OrderShippedEvent>
{
	public async Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
	{
		var items = notification.ProductIds
			.Select(productId => new ConfirmShipmentItem(productId, notification.OrderId))
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
