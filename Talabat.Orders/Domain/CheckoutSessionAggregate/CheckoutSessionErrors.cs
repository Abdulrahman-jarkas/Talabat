using ErrorOr;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate;

internal static class CheckoutSessionErrors
{
	public static Error NotFound =>
		Error.NotFound(
			code: "CheckoutSession.NotFound",
			description: "Checkout session not found.");

	public static Error NotActive =>
		Error.Conflict(
			code: "CheckoutSession.NotActive",
			description: "Checkout session is not active.");

	public static Error NotCheckedOut =>
		Error.Conflict(
			code: "CheckoutSession.NotCheckedOut",
			description: "Checkout session has not been checked out yet.");

	public static Error SessionExpired =>
		Error.Conflict(
			code: "CheckoutSession.SessionExpired",
			description: "Checkout session has expired.");

	public static Error ActiveSessionAlreadyExists =>
		Error.Conflict(
			code: "CheckoutSession.ActiveSessionAlreadyExists",
			description: "An active checkout session already exists for this customer.");

	public static Error CartEmpty =>
		Error.Validation(
			code: "CheckoutSession.CartEmpty",
			description: "Cart is empty or not found.");

	public static Error ProductsNotFound =>
		Error.NotFound(
			code: "CheckoutSession.ProductsNotFound",
			description: "No products found for the cart items.");

	public static Error ProductNotFound(Guid productId) =>
		Error.NotFound(
			code: "CheckoutSession.ProductNotFound",
			description: $"Product {productId} was not found.");

	public static Error PriceMismatch(Guid productId) =>
		Error.Conflict(
			code: "CheckoutSession.PriceMismatch",
			description: $"Price mismatch for product {productId}.");

	public static Error ReservationMissing(Guid productId) =>
		Error.Conflict(
			code: "CheckoutSession.ReservationMissing",
			description: $"Reservation is missing for product {productId}. Session is no longer valid.");

	public static Error InsufficientStock(Guid productId) =>
		Error.Conflict(
			code: "CheckoutSession.InsufficientStock",
			description: $"Insufficient stock for product {productId}.");

	public static Error AddressNotFound(Guid addressId) =>
		Error.NotFound(
			code: "CheckoutSession.AddressNotFound",
			description: $"Address {addressId} was not found for this customer.");
}
