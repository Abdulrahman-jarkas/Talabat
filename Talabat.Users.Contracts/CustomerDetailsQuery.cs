using MediatR;

namespace Talabat.Users.Contracts;

public record CustomerDetailsQuery(Guid CustomerId) : IRequest<CustomerDetailsResponse?>;

public record CustomerDetailsResponse(
	CustomerCartResponse? Cart,
	IReadOnlyList<CustomerAddressResponse> Addresses);

public record CustomerCartResponse(
	Guid MerchantId,
	IReadOnlyList<CartItemResponse> Items);

public record CartItemResponse(Guid ProductId, int Quantity);

public record CustomerAddressResponse(Guid Id, string Address);
