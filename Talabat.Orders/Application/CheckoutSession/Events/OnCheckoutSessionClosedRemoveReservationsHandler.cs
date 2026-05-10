using MediatR;
using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.Events;

internal class OnCheckoutSessionClosedRemoveReservationsHandler(
	ISender sender) : INotificationHandler<CheckoutSessionClosedEvent>
{
	public async Task Handle(CheckoutSessionClosedEvent notification, CancellationToken cancellationToken)
	{
		var removeItems = notification.ProductIds
			.Select(productId => new RemoveReservationItem(productId, notification.CheckoutSessionId))
			.ToList();

		var result = await sender.Send(new RemoveReservationRequest(removeItems), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"SessionClosed.ReservationRemovalFailed",
					$"Failed to remove reservations for session {notification.CheckoutSessionId}."),
				result.Errors);
	}
}
