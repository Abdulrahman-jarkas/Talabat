using ErrorOr;
using MediatR;

namespace Talabat.Payments.Contracts;

public record RefundPaymentRequest(Guid PaymentId) : IRequest<ErrorOr<Success>>;
