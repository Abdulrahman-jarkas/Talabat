using ErrorOr;
using MediatR;

namespace Talabat.Payments.Contracts;

public record CreatePaymentSessionRequest(int CheckoutSessionId, decimal Amount) : IRequest<ErrorOr<CreatePaymentSessionResponseDto>>;

public record class CreatePaymentSessionResponseDto
{
	public Guid PaymentId { get; init; }
	public string PaymentUrl { get; init; } = string.Empty;
}

public record PaymentSuccessedEvent(Guid PaymentId) : INotification;