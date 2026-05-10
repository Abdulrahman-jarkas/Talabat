using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record EditReservationItem(Guid ProductId);
public record EditReservationRequest(Guid CheckoutSessionId, Guid OrderId, List<EditReservationItem> Items) : IRequest<ErrorOr<Success>>;
