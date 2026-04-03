using ErrorOr;
using MediatR;

namespace Talabat.Products.Contracts;

public record AddReservationItem(Guid ProductId, Guid CheckoutSessionId, Guid UserId, int Quantity);
public record AddReservationRequest(List<AddReservationItem> Items) : IRequest<ErrorOr<Success>>;
