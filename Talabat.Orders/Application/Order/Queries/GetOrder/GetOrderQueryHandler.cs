using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Application.Order.Queries.GetOrder;

internal class GetOrderQueryHandler(IOrdersRepository ordersRepository)
    : IRequestHandler<GetOrderQuery, ErrorOr<OrderDetailsResponse>>
{
    public async Task<ErrorOr<OrderDetailsResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId, request.ShopId, request.CustomerId, cancellationToken);

        if (order is null)
            return OrderErrors.NotFound;

        return new OrderDetailsResponse(
            order.Id,
            order.CustomerId,
            order.ShopId,
            order.CheckoutSessionId,
            order.AddressId,
            order.Status.CurrentStatus.ToString(),
            order.Payment.Status.ToString(),
            order.Payment.PaymentId,
            order.Items.Select(i => new OrderItemResponse(i.ProductId, i.Quantity)).ToList());
    }
}
