using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;

namespace Talabat.Users;

internal class CustomerService : ICustomerService
{
	private readonly Guid _customerId;
	private readonly ISender _sender;
	private readonly IUsersRepository _usersRepository;

	public CustomerService(
		Guid customerId,
		ISender sender,
		IUsersRepository usersRepository)
	{
		_customerId = customerId;
		_sender = sender;
		_usersRepository = usersRepository;
	}

	public async Task<ErrorOr<Success>> AddCartItemAsync(Guid productId, int quantity, CancellationToken cancellationToken)
	{
		var customer = await _usersRepository.GetCustomerByIdAsync(_customerId, cancellationToken);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var product = await _sender.Send(new ProductQuery(productId), cancellationToken);
		if (product is null)
			return CartErrors.NoProductFoundForCartItem(productId);

		var setCartResult = customer.SetCartItem(product.Merchant, productId, quantity);
		if (setCartResult.IsError)
			return setCartResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> RemoveCartItemAsync(Guid productId, CancellationToken cancellationToken)
	{
		var customer = await _usersRepository.GetCustomerByIdAsync(_customerId, cancellationToken);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var setCartResult = customer.RemoveCartItem(productId);
		if (setCartResult.IsError)
			return setCartResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
