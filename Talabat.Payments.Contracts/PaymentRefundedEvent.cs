using MediatR;

namespace Talabat.Payments.Contracts;

public record PaymentRefundedEvent(Guid PaymentId, Guid CustomerId) : INotification;
