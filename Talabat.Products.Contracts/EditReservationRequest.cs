using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record EditReservationItem(Guid ProductId, Guid CheckoutSessionId, Guid OrderId);
public record EditReservationRequest(List<EditReservationItem> Items) : IRequest<ErrorOr<Success>>;
