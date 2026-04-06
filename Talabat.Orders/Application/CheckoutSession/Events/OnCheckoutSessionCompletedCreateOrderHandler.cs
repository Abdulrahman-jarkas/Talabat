using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Application.CheckoutSession.Events;

internal class OnCheckoutSessionCompletedCreateOrderHandler(IOrdersRepository ordersRepository)
	: INotificationHandler<CheckoutSessionCompletedEvent>
{
	public async Task Handle(CheckoutSessionCompletedEvent notification, CancellationToken cancellationToken)
	{
		var payment = Payment.Create(notification.PaymentId);

		var order = new Domain.OrderAggregate.Order(
			notification.CustomerId,
			notification.MerchantId,
			payment,
			notification.CheckoutSessionId,
			notification.AddressId,
			notification.Items.Select(i => (i.ProductId, i.Quantity)));

		await ordersRepository.AddAsync(order, cancellationToken);
		await ordersRepository.SaveChangesAsync(cancellationToken);
	}
}
