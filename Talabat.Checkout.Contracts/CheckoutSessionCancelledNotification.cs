using MediatR;

namespace Talabat.Checkout.Contracts;

public record CheckoutSessionCancelledNotification(
	Guid CheckoutSessionId,
	Guid CustomerId) : INotification;
