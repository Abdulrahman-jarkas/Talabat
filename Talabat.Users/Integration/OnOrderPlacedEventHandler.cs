using MediatR;
using Talabat.OrderProcessing.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class OnOrderPlacedEventHandler(IUsersRepository usersRepository) 
	: INotificationHandler<OnOrderPlacedEvent>
{
	public async Task Handle(OnOrderPlacedEvent notification, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerByCheckoutSessionIdAsync(
			notification.CheckoutSessionId, 
			cancellationToken);

		if (customer is null)
		{
			var error = IntegrationErrors.OnOrderPlaced.CustomerNotFound(notification.CheckoutSessionId);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		// Verify the customer ID matches (security check)
		if (customer.Id != notification.CustomerId)
		{
			var error = IntegrationErrors.OnOrderPlaced.CustomerIdMismatch(
				expectedCustomerId: customer.Id,
				actualCustomerId: notification.CustomerId);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		// Set the order ID in the checkout session
		var result = customer.SetOrderIdForCheckoutSession(
			notification.CheckoutSessionId,
			notification.OrderId);

		if (result.IsError)
		{
			var error = IntegrationErrors.OnOrderPlaced.FailedToSetOrderId(
				notification.CheckoutSessionId,
				result.Errors);
			throw new InvalidOperationException($"[{error.Code}] {error.Description}");
		}

		await usersRepository.SaveChangesAsync(cancellationToken);
	}
}
