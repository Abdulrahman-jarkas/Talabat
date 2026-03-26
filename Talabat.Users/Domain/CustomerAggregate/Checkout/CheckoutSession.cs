using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;
using Talabat.Users.Domain.CustomerAggregate.Cart;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal class CheckoutSession : Entity
{
	private readonly List<CheckoutItem> _items = new();
	public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; private set; }
	public Guid? AddressId { get; private set; } = null;
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
	}

	public static CheckoutSession Create(Guid merchantId, IEnumerable<CheckoutItem> checkoutItems, Guid? id = null)
	{
		return new CheckoutSession(merchantId, checkoutItems, id);
	}

	public void SetAddress(Guid addressId)
	{
		AddressId = addressId;
	}

	public void SetPaymentId(Guid paymentId)
	{
		PaymentId = paymentId;
	}

	/// <summary>
	/// Validates that the current session prices match the latest product prices.
	/// This ensures price integrity before payment processing.
	/// </summary>
	public ErrorOr<Success> ValidatePrices(IEnumerable<(Guid ProductId, decimal CurrentPrice)> currentPrices)
	{
		foreach (var item in _items)
		{
			var currentPrice = currentPrices.FirstOrDefault(p => p.ProductId == item.ProductId);

			if (currentPrice == default)
				return CartErrors.NoProductFoundForCartItem(item.ProductId);

			if (currentPrice.CurrentPrice != item.BasePrice)
				return CheckoutSessionErrors.PriceMismatch;
		}

		return Result.Success;
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