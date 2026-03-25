using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal class CheckoutSession : Entity
{
	private readonly List<CheckoutItem> _items = new();
	public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; private set; }
	public Guid? AddressId { get; private set; } = null;
	public PaymentType PaymentType { get; private set; }
	public CheckoutSessionStatus Status { get; private set; }
	public Guid? PaymentId { get; private set; } = null;
	public Guid? OrderId { get; private set; } = null;

	public decimal TotalPrice => _items.Sum(i => i.BasePrice * i.Quantity);

	private CheckoutSession(Guid merchantId, IEnumerable<CheckoutItem> checkoutItems, Guid? id = null)
		: base(id ?? Guid.NewGuid())
	{
		MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));

		Guard.Against.NullOrEmpty(checkoutItems, nameof(checkoutItems));
		_items.AddRange(checkoutItems);

		// Set default values
		Status = CheckoutSessionStatus.Active;
		PaymentType = PaymentType.Online; // Default to Online
	}

	public static CheckoutSession Create(Guid merchantId, IEnumerable<CheckoutItem> checkoutItems, Guid? id = null)
	{
		return new CheckoutSession(merchantId, checkoutItems, id);
	}

	public void SetAddress(Guid addressId)
	{
		AddressId = addressId;
	}

	public void SetPaymentType(PaymentType paymentType)
	{
		PaymentType = paymentType;
	}

	public void SetPaymentId(Guid paymentId)
	{
		PaymentId = paymentId;
	}

	public void SetOrderId(Guid orderId)
	{
		OrderId = orderId;
	}

	public void Complete()
	{
		Status = CheckoutSessionStatus.Completed;
	}

	// For EF Core deserialization
	private CheckoutSession()
	{
	}
}