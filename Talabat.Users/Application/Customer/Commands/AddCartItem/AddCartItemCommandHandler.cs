using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;

namespace Talabat.Users.Application.Customer.Commands.AddCartItem;

internal class AddCartItemCommandHandler(
	ISender sender,
	IUsersRepository usersRepository) : IRequestHandler<AddCartItemCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(AddCartItemCommand command, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerByIdAsync(command.CustomerId, cancellationToken);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var product = await sender.Send(new ProductQuery(command.ProductId), cancellationToken);
		if (product is null)
			return CartErrors.NoProductFoundForCartItem(command.ProductId);

		if (product.Quantity < command.Quantity)
			return CartErrors.InsufficientStock(command.ProductId);

		var setCartResult = customer.SetCartItem(product.ShopId, command.ProductId, command.Quantity);
		if (setCartResult.IsError)
			return setCartResult.Errors;

		await usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
