using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Orders.Events;
using Talabat.SharedKernal;

namespace Talabat.Orders;
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
	}

	public ErrorOr<Success> AddItem(Guid productId, int quantity, decimal basePrice)
	{
		var item = OrderItem.Create(productId, quantity, basePrice);
		_items.Add(item);

		return Result.Success;
	}

	public ErrorOr<Success> Accept()
	{
		var result = Status.Accept();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderAcceptedEvent(Id, MerchantId, CustomerId));

		return Result.Success;
	}

	public ErrorOr<Success> Reject()
	{
		var result = Status.Reject();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderRejectedEvent(Id, MerchantId, CustomerId));

		return Result.Success;
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
		if (Payment.Status == PaymentStatusValues.Unpaid)
			return OrderErrors.CannotDeliverUnpaidOrder;

		var result = Status.Deliver();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderDeliveredEvent(Id, MerchantId, CustomerId));

		return Result.Success;
	}

	//@TODO: check this business logic, cancel from the user should be after 5 minutes after placing the order if the merchant didn't accept it
	public ErrorOr<Success> Cancel()
	{
		var result = Status.Cancel();

		if (result.IsError)
			return result.Errors;

		Status = result.Value;

		_domainEvents.Add(new OrderCancelledEvent(Id, MerchantId, CustomerId));

		return Result.Success;
	}

	public ErrorOr<Success> Pay()
	{
		if (Payment.Status == PaymentStatusValues.Paid)
			return OrderErrors.InvalidPayOperation;

		var paymentRes = Payment.Pay();
		if (paymentRes.IsError)
			return paymentRes.Errors;

		_domainEvents.Add(new OrderPaidEvent(Id, MerchantId, CustomerId));

		return Result.Success;
	}

	//@TODO: check this validation logic
	public ErrorOr<Success> Refound()
	{
		if (Status.CurrentStatus != OrderStatusValues.Cancelled &&
			Payment.Status == PaymentStatusValues.Paid)
		{
			var res = Payment.Refund();

			if (res.IsError)
				return res.Errors;

			_domainEvents.Add(new OrderRefundedEvent(Id, MerchantId, CustomerId));

			return Result.Success;
		}

		return OrderErrors.InvalidRefundOperation;
	}

	// for ef core
	protected Order() { }
}