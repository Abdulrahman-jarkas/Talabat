using ErrorOr;
using MediatR;

namespace Talabat.Orders.Application.Order.Queries.GetOrder;

internal record GetOrderQuery(Guid OrderId, Guid? ShopId = null, Guid? CustomerId = null) : IRequest<ErrorOr<OrderDetailsResponse>>;

internal record OrderItemResponse(Guid ProductId, int Quantity);

internal record OrderDetailsResponse(
    Guid OrderId,
    Guid CustomerId,
    Guid ShopId,
    Guid CheckoutSessionId,
    Guid AddressId,
    string Status,
    string PaymentStatus,
    Guid PaymentId,
    IReadOnlyList<OrderItemResponse> Items);
