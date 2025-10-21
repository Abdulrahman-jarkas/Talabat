using ErrorOr;
using Talabat.OrderProcessing.Domain.Common;

namespace Talabat.OrderProcessing.Domain.OrderAggregate;

public class Order : AggregateRoot
{
	public OrderStatus Status { get; private set; }
	public PaymentMethodValues PaymentMethod { get; init; }
	public PaymentStatusValues PaymentStatus { get; private set; }

	public List<OrderItem> Items { get; private set; } = new();

	public decimal Subtotal => Items.Sum(i => i.GetTotalPrice());

	public Order(
		List<OrderItem> items,
		PaymentMethodValues paymentMethod)
	{

		if (items.Count <= 0)
			throw new InvalidDataException("Order items should not be empty");

		Items = items;

		Status = new PlacedOrderStatus();
		PaymentMethod = paymentMethod;
		PaymentStatus = paymentMethod == PaymentMethodValues.Cash ?
			PaymentStatusValues.Unpaid : PaymentStatusValues.Pending;
	}

	public ErrorOr<Success> Accept()
	{
		var result = Status.Accept();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		return Result.Success;
	}

	public ErrorOr<Success> Reject()
	{
		var result = Status.Reject();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		return Result.Success;
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
		if(PaymentStatus == PaymentStatusValues.Unpaid)
		{
			return Error.Conflict("Cannot deliver an unpaid order.");
		}

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


	public ErrorOr<Success> Pay()
	{
		if (PaymentStatus == PaymentStatusValues.Paid)
		{
			return Error.Conflict("Order is already paid.");
		}

		if (PaymentMethod == PaymentMethodValues.Cash &&
			PaymentStatus == PaymentStatusValues.Unpaid &&
			Status.CurrentStatus == OrderStatusValues.Shipped
			)
		{
			PaymentStatus = PaymentStatusValues.Paid;
		}


		return Error.Conflict("Invalid operation.");
	}

	public ErrorOr<Success> Refound()
	{
		if ((Status.CurrentStatus != OrderStatusValues.Cancelled ||
			Status.CurrentStatus != OrderStatusValues.Rejected) &&
			PaymentStatus == PaymentStatusValues.Paid)
		{
			PaymentStatus = PaymentStatusValues.Refunded;
		}

		return Error.Conflict("Invalid operation.");
	}

	// for ef core
	protected Order() { }
}