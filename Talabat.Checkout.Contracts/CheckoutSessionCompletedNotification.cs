using MediatR;

namespace Talabat.Checkout.Contracts;

public record CheckoutSessionCompletedNotification(
	Guid CheckoutSessionId,
	Guid CustomerId,
	Guid PaymentId,
	Guid OrderId) : INotification;
