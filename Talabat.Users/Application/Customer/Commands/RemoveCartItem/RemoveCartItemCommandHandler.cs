using ErrorOr;
using MediatR;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Application.Customer.Commands.RemoveCartItem;

internal class RemoveCartItemCommandHandler(
	IUsersRepository usersRepository) : IRequestHandler<RemoveCartItemCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(RemoveCartItemCommand command, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerByIdAsync(command.CustomerId, cancellationToken);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var removeResult = customer.RemoveCartItem(command.ProductId);
		if (removeResult.IsError)
			return removeResult.Errors;

		await usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
