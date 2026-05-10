using MediatR;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderPlacedEditReservationHandler(ISender sender)
	: INotificationHandler<OrderPlacedEvent>
{
	public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
	{
		var editItems = notification.ProductIds
			.Select(productId => new EditReservationItem(productId))
			.ToList();

		var result = await sender.Send(
			new EditReservationRequest(notification.CheckoutSessionId, notification.OrderId, editItems),
			cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderPlaced.ReservationEditFailed",
					$"Failed to set orderId on reservations for session {notification.CheckoutSessionId}."),
				result.Errors);
	}
}
