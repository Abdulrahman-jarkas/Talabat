using MediatR;
using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.Events;

internal class OnCheckoutSessionCheckedOutAddReservationsHandler(ISender sender)
	: INotificationHandler<CheckoutSessionCheckedOutEvent>
{
	public async Task Handle(CheckoutSessionCheckedOutEvent notification, CancellationToken cancellationToken)
	{
		var reservationItems = notification.Items
			.Select(i => new AddReservationItem(
				i.ProductId,
				notification.CheckoutSessionId,
				notification.CustomerId,
				i.Quantity))
			.ToList();

		var result = await sender.Send(new AddReservationRequest(reservationItems), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"CheckoutCheckedOut.ReservationFailed",
					$"Failed to reserve products for checkout session {notification.CheckoutSessionId}."),
				result.Errors);
	}
}
