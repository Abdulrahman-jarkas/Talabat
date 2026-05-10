using ErrorOr;
using MediatR;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Application.Customer.Commands.AddAddress;

internal class AddAddressCommandHandler(IUsersRepository usersRepository)
    : IRequestHandler<AddAddressCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(AddAddressCommand command, CancellationToken cancellationToken)
    {
        var customer = await usersRepository.GetCustomerWithAddressesByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
            return CustomerErrors.CustomerNotFound;

        customer.AddAddress(command.Address);

        await usersRepository.SaveChangesAsync(cancellationToken);

        var added = customer.Addresses.Last();
        return added.Id;
    }
}
