using ErrorOr;

namespace Talabat.Orders.Domain.OrderAggregate;

internal record OrderErrors
{
	public static Error InvalidRefundOperation =
		Error.Conflict(
			code: "Order.InvalidRefundOperation",
			description: "This order cannot be refunded.");
}