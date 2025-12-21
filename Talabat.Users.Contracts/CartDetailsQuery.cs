
using MediatR;

namespace Talabat.Users.Contracts;

public record CartDetailsQuery(Guid userId) : IRequest<CartDetailsResponse?>;

public record CartDetailsResponse(
	Guid MerchantId,
	IReadOnlyList<CartItemRespose> CartItems);

public record CartItemRespose(
	Guid productId,
	int quantity);