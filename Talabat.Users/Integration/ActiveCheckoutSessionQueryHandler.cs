using MediatR;
using Talabat.Users.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Integration;

internal class ActiveCheckoutSessionQueryHandler(IUsersRepository usersRepository) 
	: IRequestHandler<ActiveCheckoutSessionQuery, ActiveCheckoutSessionResponse?>
{
	public async Task<ActiveCheckoutSessionResponse?> Handle(
		ActiveCheckoutSessionQuery request, 
		CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerWithActiveCheckoutAsync(
			request.CustomerId, 
			cancellationToken);

		if (customer?.ActiveCheckoutSession is null)
			return null;

		var checkoutSession = customer.ActiveCheckoutSession;

		var items = checkoutSession.Items
			.Select(item => new CheckoutSessionItemResponse(
				item.ProductId,
				item.Quantity,
				item.BasePrice))
			.ToList();

		return new ActiveCheckoutSessionResponse(
			checkoutSession.Id,
			checkoutSession.MerchantId,
			checkoutSession.AddressId,
			items);
	}
}
