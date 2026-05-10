using ErrorOr;
using MediatR;

namespace Talabat.Orders.Contracts;

public record GetCheckoutSessionQuery(Guid CheckoutSessionId) : IRequest<ErrorOr<CheckoutSessionResponse>>;

public record GetCheckoutSessionByPaymentIdQuery(Guid PaymentId) : IRequest<ErrorOr<CheckoutSessionResponse>>;

public record CheckoutSessionResponse(
	Guid Id,
	Guid CustomerId,
	Guid ShopId,
	Guid AddressId,
	IReadOnlyList<CheckoutSessionItemDto> Items,
	decimal TotalPrice);

public record CheckoutSessionItemDto(
	Guid ProductId,
	decimal Price,
	int Quantity);
