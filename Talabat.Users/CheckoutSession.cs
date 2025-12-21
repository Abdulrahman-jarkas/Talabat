using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Users;

internal class CheckoutSession : Entity
{
	private readonly List<CheckoutItem> _items = new();
	public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; }
	public Guid UserId { get; }
	public Guid? AddressId { get; private set; } = null;
	public Guid? OrderId { get; private set; } = null;

	public decimal TotalPrice => _items.Sum(i => i.BasePrice * i.BasePrice);

	internal CheckoutSession(Guid userId, Guid merchantId, IEnumerable<CheckoutItem> checkoutItems)
	{
		UserId = Guard.Against.Default(userId, nameof(userId));
		MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));

		//@TODO: how check that items is valid, so the products not repeated by example
		Guard.Against.NullOrEmpty(checkoutItems, nameof(checkoutItems));
		_items.AddRange(checkoutItems);
	}

	public void SetAddress(Guid addressId)
	{
		AddressId = addressId;
	}

	private CheckoutSession()
	{
		// EF
	}
}