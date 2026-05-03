using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.CheckoutSession.Commands.CancelCheckoutSession;

internal record CancelCheckoutSessionCommand(Guid CustomerId, Guid CheckoutSessionId) : IRequest<ErrorOr<Success>>;
