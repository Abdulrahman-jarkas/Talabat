using MediatR;

namespace Talabat.Users.Contracts;

public record ActiveCheckoutSessionQuery(Guid CustomerId) : IRequest<ActiveCheckoutSessionResponse?>;

public record ActiveCheckoutSessionResponse(
	Guid CheckoutSessionId,
	Guid UserId,
	Guid MerchantId,
	Guid? AddressId,
	IReadOnlyList<CheckoutSessionItemResponse> Items);

public record CheckoutSessionItemResponse(
	Guid ProductId,
	int Quantity,
	decimal BasePrice);
