using ErrorOr;

namespace Talabat.Checkout.Domain.ProductAggregate;

internal static class ProductErrors
{
	public static Error InvalidQuantity =>
		Error.Validation(
			code: "CheckoutProduct.InvalidQuantity",
			description: "Quantity must be positive.");

	public static Error NegativeQuantity =>
		Error.Validation(
			code: "CheckoutProduct.NegativeQuantity",
			description: "Quantity cannot be negative.");

	public static Error InvalidPrice =>
		Error.Validation(
			code: "CheckoutProduct.InvalidPrice",
			description: "Price must be greater than zero.");

	public static Error InsufficientStock(Guid productId) =>
		Error.Conflict(
			code: "CheckoutProduct.InsufficientStock",
			description: $"Insufficient available quantity for product {productId}.");

	public static Error InvalidRelease(Guid productId) =>
		Error.Conflict(
			code: "CheckoutProduct.InvalidRelease",
			description: $"Cannot release more than reserved for product {productId}.");
}
