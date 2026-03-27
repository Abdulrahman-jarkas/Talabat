using ErrorOr;
using MediatR;

namespace Talabat.Checkout.Contracts;

public record GetCheckoutSessionQuery(Guid CheckoutSessionId) : IRequest<ErrorOr<CheckoutSessionResponse>>;

public record CheckoutSessionResponse(
	Guid Id,
	Guid CustomerId,
	Guid MerchantId,
	Guid AddressId,
	IReadOnlyList<CheckoutSessionItemDto> Items,
	decimal TotalPrice);

public record CheckoutSessionItemDto(
	Guid ProductId,
	decimal Price,
	int Quantity);
