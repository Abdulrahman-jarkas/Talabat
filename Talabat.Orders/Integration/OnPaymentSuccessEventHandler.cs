using Ardalis.GuardClauses;
using MediatR;
using Talabat.Payments.Contracts;

namespace Talabat.Orders.Integration;

internal class OnPaymentSuccessEventHandler(IOrdersRepository ordersRepository) : INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.GetByIdAsync(notification.PaymentId, cancellationToken);

		if (order is null)
			throw new NotFoundException(nameof(order), "");

		var acceptResult = order.Accept();

		if (acceptResult.IsError)
			throw new InvalidOperationException(string.Join(",", acceptResult.Errors.Select(e => e.Description)));

		await ordersRepository.SaveChangesAsync(cancellationToken);
	}
}
