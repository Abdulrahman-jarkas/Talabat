using MediatR;
using Talabat.Payments.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class PaymentSuccessedEventHandler(IUsersRepository usersRepository)
	: INotificationHandler<PaymentSuccessedEvent>
{
	public async Task Handle(PaymentSuccessedEvent notification, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerWithActiveCheckoutAsync(
			notification.CustomerId,
			cancellationToken);

		if (customer is null)
		{
			var error = IntegrationErrors.PaymentSuccessed.CustomerNotFound(notification.CustomerId);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		var result = customer.SetPaymentIdForActiveCheckoutSession(notification.PaymentId);

		if (result.IsError)
		{
			var error = IntegrationErrors.PaymentSuccessed.FailedToSetPaymentId(
				notification.CustomerId,
				result.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		var completeResult = customer.CompleteCheckoutSession();
		if (completeResult.IsError)
		{
			var error = IntegrationErrors.PaymentSuccessed.FailedToCompleteCheckout(
				notification.CustomerId,
				completeResult.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		await usersRepository.SaveChangesAsync(cancellationToken);
	}
}
