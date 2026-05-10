using MediatR;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Payments.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderCancelledSendRefundHandler(
	ISender sender) : INotificationHandler<OrderCancelledEvent>
{
	public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
	{
		var result = await sender.Send(new RefundPaymentRequest(notification.PaymentId), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderCancelled.RefundRequestFailed",
					$"Failed to send refund request for Order {notification.OrderId}, PaymentId {notification.PaymentId}."),
				result.Errors);
	}
}
