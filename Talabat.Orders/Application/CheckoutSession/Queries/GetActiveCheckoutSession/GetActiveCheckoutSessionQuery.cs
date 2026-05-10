using ErrorOr;
using MediatR;
using Talabat.Orders.Contracts;

namespace Talabat.Orders.Application.CheckoutSession.Queries.GetActiveCheckoutSession;

public record GetActiveCheckoutSessionQuery(Guid CustomerId) : IRequest<ErrorOr<CheckoutSessionResponse>>;
