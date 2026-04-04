using ErrorOr;
using MediatR;

namespace Talabat.Users.Application.Customer.Commands.RemoveCartItem;

internal record RemoveCartItemCommand(Guid CustomerId, Guid ProductId) : IRequest<ErrorOr<Success>>;
