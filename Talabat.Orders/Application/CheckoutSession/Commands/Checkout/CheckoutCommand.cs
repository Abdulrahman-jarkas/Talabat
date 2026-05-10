using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.CheckoutSession.Commands.Checkout;

internal record CheckoutCommand(Guid CustomerId, Guid AddressId) : IRequest<ErrorOr<CheckoutResult>>;

internal record CheckoutResult(Guid PaymentId, string PaymentUrl);
