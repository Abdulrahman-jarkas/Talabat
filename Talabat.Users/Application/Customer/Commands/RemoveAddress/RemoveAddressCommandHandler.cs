using ErrorOr;
using MediatR;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Application.Customer.Commands.RemoveAddress;

internal class RemoveAddressCommandHandler(IUsersRepository usersRepository)
    : IRequestHandler<RemoveAddressCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RemoveAddressCommand command, CancellationToken cancellationToken)
    {
        var customer = await usersRepository.GetCustomerWithAddressesByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
            return CustomerErrors.CustomerNotFound;

        var result = customer.RemoveAddress(command.AddressId);
        if (result.IsError)
            return result.Errors;

        await usersRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
