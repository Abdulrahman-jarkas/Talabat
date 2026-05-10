using MediatR;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;

namespace Talabat.Orders.Application.Order.Events;

internal class OnOrderCancelledRemoveReservationsHandler(
    ISender sender) : INotificationHandler<OrderCancelledEvent>
{
    public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RemoveReservationRequest(
                notification.ProductIds.Select(pid => new RemoveReservationItem(pid, notification.CheckoutSessionId)).ToList()),
            cancellationToken);

        if (result.IsError)
            throw new EventualConsistencyException(
                EventualConsistencyError.From(
                    "OrderCancelled.ReservationRemovalFailed",
                    $"Failed to remove reservations for Order {notification.OrderId}."),
                result.Errors);
    }
}
