using ErrorOr;
using MediatR;

namespace Talabat.Users.Application.Customer.Commands.AddCartItem;

internal record AddCartItemCommand(Guid CustomerId, Guid ProductId, int Quantity) : IRequest<ErrorOr<Success>>;
