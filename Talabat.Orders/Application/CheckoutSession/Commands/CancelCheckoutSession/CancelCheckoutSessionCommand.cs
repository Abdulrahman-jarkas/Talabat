using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.CheckoutSession.Commands.CancelCheckoutSession;

internal record CancelCheckoutSessionCommand(Guid CheckoutSessionId) : IRequest<ErrorOr<Success>>;
