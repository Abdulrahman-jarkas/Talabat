using MediatR;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class OnOrderPlacedEventHandler(IUsersRepository usersRepository) 
	: INotificationHandler<OrderPlacedEvent>
{
	public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerDetailsAsync(
			notification.CustomerId, 
			cancellationToken);

		if (customer is null)
			return;

		// Reset cart and checkout session
		customer.ResetCartAndCheckoutSession();

		await usersRepository.SaveChangesAsync(cancellationToken);
	}
}
