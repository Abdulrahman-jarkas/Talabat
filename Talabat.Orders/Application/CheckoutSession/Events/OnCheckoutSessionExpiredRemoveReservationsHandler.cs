// we don't need to remove reservation on session expired, because we don't reserve products until the session is checked out. So if a session expires without being checked out, there are no reservations to remove.
//using MediatR;
//using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
//using Talabat.Products.Contracts;
//using Talabat.SharedKernal;

//namespace Talabat.Orders.Application.CheckoutSession.Events;

//internal class OnCheckoutSessionExpiredRemoveReservationsHandler(
//	ISender sender) : INotificationHandler<CheckoutSessionExpiredEvent>
//{
//	public async Task Handle(CheckoutSessionExpiredEvent notification, CancellationToken cancellationToken)
//	{
//		var removeItems = notification.ProductIds
//			.Select(productId => new RemoveReservationItem(productId, notification.CheckoutSessionId))
//			.ToList();

//		var result = await sender.Send(new RemoveReservationRequest(removeItems), cancellationToken);

//		if (result.IsError)
//			throw new EventualConsistencyException(
//				EventualConsistencyError.From(
//					"SessionExpired.ReservationRemovalFailed",
//					$"Failed to remove reservations for session {notification.CheckoutSessionId}."),
//				result.Errors);
//	}
//}
