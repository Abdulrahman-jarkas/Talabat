using MediatR;
using Talabat.Payments.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class PaymentSuccessedEventHandler(IUsersRepository usersRepository)
	: INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerDetailsAsync(
			notification.CustomerId,
			cancellationToken);

		if (customer is null)
			return;

		var result = customer.SetPaymentId(notification.PaymentId);

		if (result.IsError)
			return;

		var completeResult = customer.CompleteCheckoutSession();
		if (completeResult.IsError)
			return;

		await usersRepository.SaveChangesAsync(cancellationToken);
	}
}
