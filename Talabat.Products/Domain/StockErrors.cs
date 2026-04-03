using ErrorOr;

namespace Talabat.Products.Domain;

internal static class StockErrors
{
	public static Error InvalidQuantity =>
		Error.Validation(
			"Stock.InvalidQuantity",
			"Quantity must be positive.");

	public static Error NegativeQuantity =>
		Error.Validation(
			"Stock.NegativeQuantity",
			"Quantity cannot be negative.");

	public static Error InsufficientStock =>
		Error.Conflict(
			"Stock.InsufficientStock",
			"Insufficient stock to fulfill reservation.");

	public static Error ReservationNotFound(Guid checkoutSessionId) =>
		Error.NotFound(
			"Stock.ReservationNotFound",
			$"Reservation for checkout session '{checkoutSessionId}' was not found.");

	public static Error OrderAlreadySet(Guid checkoutSessionId) =>
		Error.Conflict(
			"Stock.OrderAlreadySet",
			$"Order has already been set for reservation '{checkoutSessionId}'.");

	public static Error ReservationAlreadyRemoved(Guid checkoutSessionId) =>
		Error.Conflict(
			"Stock.ReservationAlreadyRemoved",
			$"Reservation for checkout session '{checkoutSessionId}' was not found — may have already been removed.");

	public static Error ShipmentReservationNotFound(Guid orderId) =>
		Error.Conflict(
			"Stock.ShipmentReservationNotFound",
			$"No reservation found for order '{orderId}' — may have already been deducted.");

	public static Error CannotReduceQuantity =>
		Error.Conflict(
			"Stock.CannotReduceQuantity",
			"Cannot reduce quantity below paid reservations.");
}
