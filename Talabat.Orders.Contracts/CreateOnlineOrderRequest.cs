using ErrorOr;
using MediatR;

namespace Talabat.Orders.Contracts;

public record CreateOnlineOrderRequest(
	Guid CustomerId,
	Guid MerchantId,
	Guid CheckoutSessionId,
	Guid AddressId,
	List<OrderItemDto> Items,
	Guid PaymentId
	) : IRequest<ErrorOr<Guid>>;

public record CreateCashOrderRequest(
	Guid CustomerId,
	Guid MerchantId,
	Guid CheckoutSessionId,
	Guid AddressId,
	List<OrderItemDto> Items
	) : IRequest<ErrorOr<Guid>>;

public record OrderItemDto(
	Guid ProductId,
	int Quantity,
	decimal BasePrice);