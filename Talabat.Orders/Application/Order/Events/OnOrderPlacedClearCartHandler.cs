using MediatR;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.SharedKernal;
using Talabat.Users.Contracts;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderPlacedClearCartHandler(
	ISender sender) : INotificationHandler<OrderPlacedEvent>
{
	public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
	{
		var result = await sender.Send(new ClearCartRequest(notification.CustomerId), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderPlaced.ClearCartFailed",
					$"Failed to clear cart for Customer {notification.CustomerId} after Order {notification.OrderId}."),
				result.Errors);
	}
}
