using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.CheckoutSession.Commands.CreateCheckoutSession;

internal record CreateCheckoutSessionCommand(Guid CustomerId) : IRequest<ErrorOr<Success>>;
