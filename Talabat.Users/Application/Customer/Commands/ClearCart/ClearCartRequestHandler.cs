using ErrorOr;
using MediatR;
using Talabat.Users.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Application.Customer.Commands.ClearCart;

internal class ClearCartRequestHandler(IUsersRepository usersRepository)
	: IRequestHandler<ClearCartRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(ClearCartRequest request, CancellationToken cancellationToken)
	{
		var customer = await usersRepository.GetCustomerByIdAsync(request.CustomerId, cancellationToken);

		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var result = customer.ResetCart(raiseEvent: false);
		if (result.IsError)
			return result.Errors;

		await usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
