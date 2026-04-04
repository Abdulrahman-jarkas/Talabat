using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.CheckoutSession.Commands.CreateCheckoutSession;

internal record CreateCheckoutSessionCommand(Guid CustomerId, Guid AddressId) : IRequest<ErrorOr<Success>>;
