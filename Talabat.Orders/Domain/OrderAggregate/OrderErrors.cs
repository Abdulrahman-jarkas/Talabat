using ErrorOr;

namespace Talabat.Orders.Domain.OrderAggregate;

internal record OrderErrors
{
	public static Error CannotDeliverUnpaidOrder =>
		Error.Conflict(
			code: "Order.CannotDeliverUnpaidOrder",
			description: "Cannot deliver an unpaid order.");

	public static Error InvalidPayOperation =
		Error.Conflict(
			code: "Order.InvalidPayOperation",
			description: "This order is already paid.");

	public static Error InvalidRefundOperation =
		Error.Conflict(
			code: "Order.InvalidRefundOperation",
			description: "This order cannot be refunded.");
}