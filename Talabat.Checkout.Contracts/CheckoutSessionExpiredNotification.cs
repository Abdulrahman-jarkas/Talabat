using MediatR;

namespace Talabat.Checkout.Contracts;

public record CheckoutSessionExpiredNotification(
	Guid CheckoutSessionId,
	Guid CustomerId) : INotification;
