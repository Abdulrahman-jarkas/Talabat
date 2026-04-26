using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Orders.Domain.OrderAggregate.Events;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate;

internal class Order : AggregateRoot
{
	public Guid CustomerId { get; init; }
	public Guid ShopId { get; init; }
	public Guid CheckoutSessionId { get; init; }
	public Guid AddressId { get; init; }

	public OrderStatus Status { get; private set; }
	public Payment Payment { get; private set; }

	private List<OrderItem> _items = new();
	public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

	internal Order(
		Guid customerId,
		Guid shopId,
		Payment payment,
		Guid checkoutSessionId,
		Guid addressId,
		IEnumerable<(Guid ProductId, int Quantity)> items) : base(Guid.NewGuid())
	{
		CheckoutSessionId = Guard.Against.Default(checkoutSessionId);
		AddressId = Guard.Against.Default(addressId);
		CustomerId = Guard.Against.Default(customerId);
		ShopId = Guard.Against.Default(shopId);
		Status = OrderStatus.FromStatusValue(OrderStatusValues.Placed);
		Payment = Guard.Against.Null(payment);

		foreach (var item in items)
			_items.Add(OrderItem.Create(item.ProductId, item.Quantity));

		_domainEvents.Add(new OrderPlacedEvent(Id, CustomerId, ShopId, CheckoutSessionId,
			_items.Select(i => i.ProductId).ToList()));
	}

	public ErrorOr<Success> Ship()
	{
		var result = Status.Ship();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderShippedEvent(Id, ShopId, CustomerId, _items.Select(i => i.ProductId).ToList()));

		return Result.Success;
	}

	public ErrorOr<Success> Deliver()
	{
		var result = Status.Deliver();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderDeliveredEvent(Id, ShopId, CustomerId));

		return Result.Success;
	}

	public ErrorOr<Success> Cancel()
	{
		var result = Status.Cancel();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderCancelledEvent(Id, ShopId, CustomerId, Payment.PaymentId));

		return Result.Success;
	}

    public ErrorOr<Success> Refund()
    {
        if (Status.CurrentStatus != OrderStatusValues.Cancelled)
            return OrderErrors.InvalidRefundOperation;

        var res = Payment.Refund();

        if (res.IsError)
            return res.Errors;

		_domainEvents.Add(new OrderRefundedEvent(Id, ShopId, CustomerId));

		return Result.Success;
	}

	// for ef core
	protected Order() { }
}