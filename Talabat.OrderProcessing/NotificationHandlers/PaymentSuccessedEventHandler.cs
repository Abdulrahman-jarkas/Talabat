using MediatR;
using Talabat.OrderProcessing.Application.Services;
using Talabat.OrderProcessing.Data.Repositories;
using Talabat.Payments.Contracts;

namespace Talabat.OrderProcessing.NotificationHandlers;

public class PaymentSuccessedEventHandler(IOrderService orderService, ICheckoutSessionsRepository checkoutSessionsRepository) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		var checkoutSession = await checkoutSessionsRepository.GetCheckoutSessionByPaymentIdAsync(notification.PaymentId);

		if (checkoutSession is null)
			throw new InvalidDataException("Checkout session not found for the given payment id");

		await orderService.CreateOrderFromCheckoutSessionAsync(checkoutSession, cancellationToken);
	}
}
