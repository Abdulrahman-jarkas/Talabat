using MediatR;

namespace Talabat.Payments.Contracts;

public record PaymentFailedEvent(Guid PaymentId, Guid CustomerId, string Reason) : INotification;
