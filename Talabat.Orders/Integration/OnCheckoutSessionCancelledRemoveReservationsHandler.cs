using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Integration;

internal class OnCheckoutSessionCancelledRemoveReservationsHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ISender sender) : INotificationHandler<CheckoutSessionCancelledEvent>
{
	public async Task Handle(CheckoutSessionCancelledEvent notification, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByIdAsync(notification.CheckoutSessionId, cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"SessionCancelled.SessionNotFound",
					$"Checkout session {notification.CheckoutSessionId} not found during cancellation."));

		var removeItems = session.Items
			.Select(i => new RemoveReservationItem(i.ProductId, session.Id))
			.ToList();

		var result = await sender.Send(new RemoveReservationRequest(removeItems), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"SessionCancelled.ReservationRemovalFailed",
					$"Failed to remove reservations for session {session.Id}."),
				result.Errors);
	}
}
