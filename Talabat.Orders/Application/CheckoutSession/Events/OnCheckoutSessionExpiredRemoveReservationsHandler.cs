using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.CheckoutSession.Events;

internal class OnCheckoutSessionExpiredRemoveReservationsHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	ISender sender) : INotificationHandler<CheckoutSessionExpiredEvent>
{
	public async Task Handle(CheckoutSessionExpiredEvent notification, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByIdAsync(notification.CheckoutSessionId, cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"SessionExpired.SessionNotFound",
					$"Checkout session {notification.CheckoutSessionId} not found during expiration."));

		var removeItems = session.Items
			.Select(i => new RemoveReservationItem(i.ProductId, session.Id))
			.ToList();

		var result = await sender.Send(new RemoveReservationRequest(removeItems), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"SessionExpired.ReservationRemovalFailed",
					$"Failed to remove reservations for session {session.Id}."),
				result.Errors);
	}
}
