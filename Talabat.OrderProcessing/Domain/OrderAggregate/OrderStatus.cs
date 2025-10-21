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
			_ => throw new NotImplementedException("Order status not implemented."),
		};
	}

	public virtual ErrorOr<AcceptedOrderStatus> Accept() => throw new NotImplementedException();
	public virtual ErrorOr<RejectedOrderStatus> Reject() => throw new NotImplementedException();
	public virtual ErrorOr<ShippedOrderStatus> Ship() => throw new NotImplementedException();
	public virtual ErrorOr<DeliveredOrderStatus> Deliver() => throw new NotImplementedException();
	public virtual ErrorOr<CancelledOrderStatus> Cancel() => throw new NotImplementedException();
}

public class PlacedOrderStatus : OrderStatus
{
	public PlacedOrderStatus() : base(OrderStatusValues.Placed)
	{
	}

	public override ErrorOr<AcceptedOrderStatus> Accept()
	{
		return new AcceptedOrderStatus();
	}

	public override ErrorOr<RejectedOrderStatus> Reject()
	{
		return new RejectedOrderStatus();
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Order must be accepted before it can be shipped.");
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

public class AcceptedOrderStatus : OrderStatus
{
	public AcceptedOrderStatus() : base(OrderStatusValues.Accepted)
	{
	}

	public override ErrorOr<AcceptedOrderStatus> Accept()
	{
		return Error.Conflict("Order is already accepted.");
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		//@TODO: Add business logic to determine if cancellation is allowed
		return Error.Conflict("Cannot cancel an accepted order.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Order must be shipped before it can be delivered.");
	}

	public override ErrorOr<RejectedOrderStatus> Reject()
	{
		return Error.Conflict("Cannot reject an accepted order.");
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return new ShippedOrderStatus();
	}
}

public class RejectedOrderStatus : OrderStatus
{
	public RejectedOrderStatus() : base(OrderStatusValues.Rejected)
	{
	}

	public override ErrorOr<AcceptedOrderStatus> Accept()
	{
		return Error.Conflict("Cannot accept a rejected order.");
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Conflict("Cannot cancel a rejected order.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Cannot deliver a rejected order.");
	}

	public override ErrorOr<RejectedOrderStatus> Reject()
	{
		return Error.Conflict("Order is already rejected.");
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Cannot ship a rejected order.");
	}
}

public class CancelledOrderStatus : OrderStatus
{
	public CancelledOrderStatus() : base(OrderStatusValues.Cancelled)
	{
	}

	public override ErrorOr<AcceptedOrderStatus> Accept()
	{
		return Error.Conflict("Cannot accept a cancelled order.");
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Conflict("Order is already cancelled.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Cannot deliver a cancelled order.");
	}

	public override ErrorOr<RejectedOrderStatus> Reject()
	{
		return Error.Conflict("Cannot reject a cancelled order.");
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Cannot ship a cancelled order.");
	}
}

public class ShippedOrderStatus : OrderStatus
{
	public ShippedOrderStatus() : base(OrderStatusValues.Shipped)
	{
	}
	public override ErrorOr<AcceptedOrderStatus> Accept()
	{
		return Error.Conflict("Order is already shipped.");
	}
	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Validation("Cannot cancel a shipped order.");
	}
	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return new DeliveredOrderStatus();
	}
	public override ErrorOr<RejectedOrderStatus> Reject()
	{
		return Error.Conflict("Cannot reject a shipped order.");
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

	public override ErrorOr<AcceptedOrderStatus> Accept()
	{
		return Error.Conflict("Order is already delivered.");
	}

	public override ErrorOr<CancelledOrderStatus> Cancel()
	{
		return Error.Conflict("Cannot cancel a delivered order.");
	}

	public override ErrorOr<DeliveredOrderStatus> Deliver()
	{
		return Error.Conflict("Order is already delivered.");
	}

	public override ErrorOr<RejectedOrderStatus> Reject()
	{
		return Error.Conflict("Cannot reject a delivered order.");
	}

	public override ErrorOr<ShippedOrderStatus> Ship()
	{
		return Error.Conflict("Order is already delivered.");
	}
}