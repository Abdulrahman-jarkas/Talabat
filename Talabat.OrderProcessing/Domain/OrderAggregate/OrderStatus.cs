using ErrorOr;

namespace Talabat.OrderProcessing.Domain.OrderAggregate;

public class OrderStatus
{
	public OrderStatusValues CurrentStatus { get; private set; }

	protected OrderStatus(OrderStatusValues currentStatus)
	{
		CurrentStatus = currentStatus;
	}

	public static OrderStatus FromStatusValue(OrderStatusValues currentStatus)
	{
		return currentStatus switch
		{
			OrderStatusValues.Placed => new PlacedOrderStatus(),
			OrderStatusValues.Shipped => new ShippedOrderStatus(),
			OrderStatusValues.Delivered => new DeliveredOrderStatus(),
			OrderStatusValues.Cancelled => new CancelledOrderStatus(),
			_ => throw new NotImplementedException("Order status not implemented."),
		};
	}

	public virtual ErrorOr<ShippedOrderStatus> Ship() => throw new NotImplementedException();
	public virtual ErrorOr<DeliveredOrderStatus> Deliver() => throw new NotImplementedException();
	public virtual ErrorOr<CancelledOrderStatus> Cancel() => throw new NotImplementedException();
}

public class PlacedOrderStatus : OrderStatus
{
	public PlacedOrderStatus() : base(OrderStatusValues.Placed)
	{
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return new ShippedOrderStatus();
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Order must be shipped before it can be delivered.");
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return new CancelledOrderStatus();
	}
}

public class CancelledOrderStatus : OrderStatus
{
	public CancelledOrderStatus() : base(OrderStatusValues.Cancelled)
	{
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Cannot ship a cancelled order.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Cannot deliver a cancelled order.");
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Conflict("Order is already cancelled.");
	}
}

public class ShippedOrderStatus : OrderStatus
{
	public ShippedOrderStatus() : base(OrderStatusValues.Shipped)
	{
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Validation("Cannot cancel a shipped order.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return new DeliveredOrderStatus();
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Order is already shipped.");
	}
}

public class DeliveredOrderStatus : OrderStatus
{
	public DeliveredOrderStatus() : base(OrderStatusValues.Delivered)
	{
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Conflict("Cannot cancel a delivered order.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Order is already delivered.");
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Order is already delivered.");
	}
}