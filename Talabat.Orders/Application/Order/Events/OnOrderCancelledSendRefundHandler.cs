using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Payments.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderCancelledSendRefundHandler(
	IOrdersRepository ordersRepository,
	ISender sender) : INotificationHandler<OrderCancelledEvent>
{
	public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.GetByIdAsync(notification.OrderId, cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderCancelled.OrderNotFound",
					$"Order {notification.OrderId} not found during refund initiation."));

		var result = await sender.Send(new RefundPaymentRequest(order.Payment.PaymentId), cancellationToken);

		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"OrderCancelled.RefundRequestFailed",
					$"Failed to send refund request for Order {notification.OrderId}, PaymentId {order.Payment.PaymentId}."),
				result.Errors);
	}
}
