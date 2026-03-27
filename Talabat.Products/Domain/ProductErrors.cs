using ErrorOr;

namespace Talabat.Products.Domain;

internal static class ProductErrors
{
	public static Error NotFound(Guid productId) =>
		Error.NotFound(
			"Product.NotFound",
			$"Product '{productId}' was not found.");

	public static Error InsufficientStock(Guid productId) =>
		Error.Conflict(
			"Product.InsufficientStock",
			$"Insufficient stock for product '{productId}'.");
}
