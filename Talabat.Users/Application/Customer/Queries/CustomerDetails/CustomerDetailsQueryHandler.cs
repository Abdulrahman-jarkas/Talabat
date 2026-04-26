using MediatR;
using Talabat.Users.Contracts;
using Talabat.Users.Data.Repositories;

namespace Talabat.Users.Application.Customer.Queries.CustomerDetails;

internal class CustomerDetailsQueryHandler(IUsersRepository usersRepository) : IRequestHandler<CustomerDetailsQuery, CustomerDetailsResponse?>
{
	public async Task<CustomerDetailsResponse?> Handle(CustomerDetailsQuery request, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerWithAddressesByIdAsync(request.CustomerId, cancellationToken);
		if (customer is null)
			return null;

		var cart = customer.Cart is null ? null : new CustomerCartResponse(
			customer.Cart.ShopId,
			customer.Cart.Items.Select(i => new CartItemResponse(i.ProductId, i.Quantity)).ToList());

		var addresses = customer.Addresses
			.Select(a => new CustomerAddressResponse(a.Id, a.Address))
			.ToList();

		return new CustomerDetailsResponse(cart, addresses);
	}
}
