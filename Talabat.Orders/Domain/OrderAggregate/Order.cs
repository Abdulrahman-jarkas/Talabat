using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate;

internal class Order : AggregateRoot
{
    public Guid CustomerId { get; init; }
    public Guid MerchantId { get; init; }
    public Guid CheckoutSessionId { get; init; }
    public Guid AddressId { get; init; }

    public OrderStatus Status { get; private set; }
    public Payment Payment { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    internal Order(
        Guid customerId,
        Guid merchantId,
        Payment payment,
        Guid checkoutSession,
        Guid addressId)
    {
        CheckoutSessionId = Guard.Against.Default(checkoutSession);
        AddressId = Guard.Against.Default(addressId);
        CustomerId = Guard.Against.Default(customerId);
        MerchantId = Guard.Against.Default(merchantId);
        Status = OrderStatus.FromStatusValue(OrderStatusValues.Placed);
        Payment = Guard.Against.Null(payment);

        _domainEvents.Add(new OrderPlacedEvent(Id, CustomerId, MerchantId, CheckoutSessionId));
    }

    public ErrorOr<Success> Ship()
    {
        var result = Status.Ship();

        if (result.IsError)
            return result.Errors;

        Status = result.Value;

        _domainEvents.Add(new OrderShippedEvent(Id, MerchantId, CustomerId));

        return Result.Success;
    }

    public ErrorOr<Success> Deliver()
    {
        var result = Status.Deliver();

        if (result.IsError)
            return result.Errors;

        Status = result.Value;

        _domainEvents.Add(new OrderDeliveredEvent(Id, MerchantId, CustomerId));

        return Result.Success;
    }

    public ErrorOr<Success> Cancel()
    {
        var result = Status.Cancel();

        if (result.IsError)
            return result.Errors;

        Status = result.Value;

        _domainEvents.Add(new OrderCancelledEvent(Id, MerchantId, CustomerId));

        return Result.Success;
    }

    public ErrorOr<Success> AddItem(Guid productId, int quantity, decimal basePrice)
    {
        var item = OrderItem.Create(productId, quantity, basePrice);
        _items.Add(item);
        return Result.Success;
    }

    public ErrorOr<Success> Refund()
    {
        if (Status.CurrentStatus != OrderStatusValues.Cancelled)
            return OrderErrors.InvalidRefundOperation;

        var res = Payment.Refund();

        if (res.IsError)
            return res.Errors;

        _domainEvents.Add(new OrderRefundedEvent(Id, MerchantId, CustomerId));

        return Result.Success;
    }

    // for ef core
    protected Order() { }
}