using MediatR;

namespace Talabat.OrderProcessing.Contracts;

public record OnOrderPlacedEvent(
	Guid OrderId,
	Guid PaymentId,
	Guid CheckoutSessionId,
	Guid CustomerId) : INotification;
