using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record RemoveReservationItem(Guid ProductId, Guid CheckoutSessionId);
public record RemoveReservationRequest(List<RemoveReservationItem> Items) : IRequest<ErrorOr<Success>>;
