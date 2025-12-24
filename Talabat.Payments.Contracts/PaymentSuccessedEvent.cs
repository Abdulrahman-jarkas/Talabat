using MediatR;

namespace Talabat.Payments.Contracts;

public record PaymentSuccessedEvent(Guid PaymentId, Guid CustomerId) : INotification;