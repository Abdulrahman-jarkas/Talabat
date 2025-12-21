using MediatR;
using Talabat.Users.Contracts;

namespace Talabat.Users.Integration;

internal class CartDetailsQueryHandler(IUsersRepository usersRepository) : IRequestHandler<CartDetailsQuery, CartDetailsResponse?>
{
	public async Task<CartDetailsResponse?> Handle(CartDetailsQuery request, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerAsync(request.userId);
		if (customer is null || customer.Cart is null)
			return null;

		var cartResponse = new CartDetailsResponse(
			customer.Cart.MerchantId,
			customer.Cart.Items.Select(ci => new CartItemRespose(
				ci.ProductId,
				ci.Quantity)).ToList());

		return cartResponse;
	}
}

