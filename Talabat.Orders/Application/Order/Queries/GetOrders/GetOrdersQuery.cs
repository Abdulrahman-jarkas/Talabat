using MediatR;

namespace Talabat.Orders.Application.Order.Queries.GetOrders;

internal record GetOrdersQuery(Guid? ShopId = null, Guid? CustomerId = null) : IRequest<List<OrderSummaryResponse>>;

internal record OrderSummaryResponse(
    Guid OrderId,
    Guid CustomerId,
    Guid ShopId,
    string Status,
    string PaymentStatus);
