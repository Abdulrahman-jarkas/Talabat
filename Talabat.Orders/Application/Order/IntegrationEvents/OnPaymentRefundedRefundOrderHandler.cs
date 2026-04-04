using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.IntegrationEvents;

internal class OnPaymentRefundedRefundOrderHandler(
	ICheckoutSessionRepository checkoutSessionRepository,
	IOrdersRepository ordersRepository) : INotificationHandler<PaymentRefundedEvent>
{
	public async Task Handle(PaymentRefundedEvent notification, CancellationToken cancellationToken)
	{
		var session = await checkoutSessionRepository.GetByPaymentIdAsync(notification.PaymentId, cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentRefunded.SessionNotFound",
					$"No checkout session found for PaymentId {notification.PaymentId}."));

		var order = await ordersRepository.GetByCheckoutSessionIdAsync(session.Id, cancellationToken)
			?? throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentRefunded.OrderNotFound",
					$"No order found for CheckoutSession {session.Id}."));

		var result = order.Refund();
		if (result.IsError)
			throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"PaymentRefunded.RefundFailed",
					$"Failed to refund Order {order.Id}."),
				result.Errors);

		await ordersRepository.SaveChangesAsync(cancellationToken);
	}
}
