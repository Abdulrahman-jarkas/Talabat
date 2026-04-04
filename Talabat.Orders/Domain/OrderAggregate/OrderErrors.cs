using ErrorOr;

namespace Talabat.Orders.Domain.OrderAggregate;

internal record OrderErrors
{
	public static Error NotFound =
		Error.NotFound(
			code: "Order.NotFound",
			description: "Order was not found.");

	public static Error InvalidRefundOperation =
		Error.Conflict(
			code: "Order.InvalidRefundOperation",
			description: "This order cannot be refunded.");
}