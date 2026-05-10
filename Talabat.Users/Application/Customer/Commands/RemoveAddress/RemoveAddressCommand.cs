using ErrorOr;
using MediatR;

namespace Talabat.Users.Application.Customer.Commands.RemoveAddress;

internal record RemoveAddressCommand(Guid CustomerId, Guid AddressId) : IRequest<ErrorOr<Success>>;
