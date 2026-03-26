using MediatR;
using Talabat.Payments.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class PaymentFailedEventHandler(IUsersRepository usersRepository)
	: INotificationHandler<PaymentFailedEvent>
{
	public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerWithActiveCheckoutAsync(
			notification.CustomerId,
			cancellationToken);

		if (customer is null)
		{
			var error = IntegrationErrors.PaymentFailed.CustomerNotFound(notification.CustomerId);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		var result = customer.CancelCheckoutSession();

		if (result.IsError)
		{
			var error = IntegrationErrors.PaymentFailed.FailedToCancelCheckout(
				notification.CustomerId,
				result.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		await usersRepository.SaveChangesAsync(cancellationToken);
	}
}
