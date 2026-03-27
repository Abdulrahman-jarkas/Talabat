using ErrorOr;
using MediatR;

namespace Talabat.Orders.Contracts;

public record CreateOrderRequest(
	Guid CustomerId,
	Guid MerchantId,
	Guid CheckoutSessionId,
	Guid AddressId,
	Guid PaymentId,
	IReadOnlyList<CreateOrderItemDto> Items) : IRequest<ErrorOr<CreateOrderResponse>>;

public record CreateOrderItemDto(Guid ProductId, int Quantity, decimal BasePrice);

public record CreateOrderResponse(Guid OrderId);
