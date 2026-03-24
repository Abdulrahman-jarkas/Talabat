using MediatR;
using Talabat.ProductsManagement.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class ProductDeletedEventHandler(IUsersRepository usersRepository) : INotificationHandler<ProductDeletedEvent>
{
	public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
	{
		var customersWithProductInCart = await usersRepository.GetCustomersWithProductInCartAsync(notification.ProductId, cancellationToken);

		if (customersWithProductInCart is null || !customersWithProductInCart.Any())
			return;

		foreach (var customer in customersWithProductInCart)
		{
			if (customer.ActiveCheckoutSession is not null)
			{
				customer.ResetActiveCheckoutSession();
			}
		}

		await usersRepository.SaveChangesAsync(cancellationToken);
	}
}
