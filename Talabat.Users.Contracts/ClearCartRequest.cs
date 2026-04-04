using ErrorOr;
using MediatR;

namespace Talabat.Users.Contracts;

public record ClearCartRequest(Guid CustomerId) : IRequest<ErrorOr<Success>>;
