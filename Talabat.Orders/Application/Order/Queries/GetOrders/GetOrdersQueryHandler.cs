using MediatR;
using Talabat.Orders.Data.Repositories;

namespace Talabat.Orders.Application.Order.Queries.GetOrders;

internal class GetOrdersQueryHandler(IOrdersRepository ordersRepository)
    : IRequestHandler<GetOrdersQuery, List<OrderSummaryResponse>>
{
    public async Task<List<OrderSummaryResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await ordersRepository.GetOrdersAsync(request.ShopId, request.CustomerId, cancellationToken);

        return orders
            .Select(o => new OrderSummaryResponse(
                o.Id,
                o.CustomerId,
                o.ShopId,
                o.Status.CurrentStatus.ToString(),
                o.Payment.Status.ToString()))
            .ToList();
    }
}
