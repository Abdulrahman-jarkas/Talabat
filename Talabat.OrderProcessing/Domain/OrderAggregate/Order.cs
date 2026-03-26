using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.OrderProcessing.Domain.OrderAggregate;

public class Order : AggregateRoot
{
	public OrderStatus Status { get; private set; }
	public PaymentStatusValues PaymentStatus { get; private set; }
	//public decimal ServiceFees { get; private set; }
	public Guid PaymentId { get; init; }

	public List<OrderItem> Items { get; private set; } = new();

	public decimal Subtotal => Items.Sum(i => i.GetTotalPrice());
	//public decimal Total => Subtotal + ServiceFees;
	public decimal Total => Subtotal;



	public Order(
		List<OrderItem> items,
		Guid paymentId
		//decimal serviceFees
		)
	{

		if (items.Count <= 0)
			throw new InvalidDataException("Order items should not be empty");

		Items = items;

		Status = new PlacedOrderStatus();
		PaymentStatus = PaymentStatusValues.Paid;

		//ServiceFees = serviceFees;
		PaymentId = paymentId;
	}

	public ErrorOr<Success> Ship()
	{
		var result = Status.Ship();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		return Result.Success;
	}

	public ErrorOr<Success> Deliver()
	{
		var result = Status.Deliver();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		return Result.Success;
	}

	public ErrorOr<Success> Cancel()
	{
		var result = Status.Cancel();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		return Result.Success;
	}


	public ErrorOr<Success> Refund()
	{
		if (Status.CurrentStatus == OrderStatusValues.Cancelled &&
			PaymentStatus == PaymentStatusValues.Paid)
		{
			PaymentStatus = PaymentStatusValues.Refunded;
			return Result.Success;
		}

		return Error.Conflict("Invalid operation.");
	}

	// for ef core
	protected Order() { }
}