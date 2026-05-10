using ErrorOr;
using MediatR;

namespace Talabat.Users.Application.Customer.Commands.AddAddress;

internal record AddAddressCommand(Guid CustomerId, string Address) : IRequest<ErrorOr<Guid>>;
