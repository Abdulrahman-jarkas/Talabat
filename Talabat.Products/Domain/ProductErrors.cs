using ErrorOr;

namespace Talabat.Products.Domain;

internal static class ProductErrors
{
	public static Error NotFound(Guid productId) =>
		Error.NotFound(
			"Product.NotFound",
			$"Product '{productId}' was not found.");

	public static Error HasActiveReservations(Guid productId) =>
		Error.Conflict(
			"Product.HasActiveReservations",
			$"Product '{productId}' cannot be deleted because it has active reservations.");
}
