using Ardalis.GuardClauses;
using System.Text.Json.Serialization;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal class CheckoutSession : ValueObject
{
	public Guid Id { get; }

	private readonly List<CheckoutItem> _items = new();
	public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; }
	public Guid UserId { get; }
	public Guid? AddressId { get; private set; } = null;
	public Guid? OrderId { get; private set; } = null;

	public decimal TotalPrice => _items.Sum(i => i.BasePrice * i.Quantity);

	private CheckoutSession(Guid userId, Guid merchantId, IEnumerable<CheckoutItem> checkoutItems, Guid? id = null)
	{
		Id = id ?? Guid.NewGuid();
		UserId = Guard.Against.Default(userId, nameof(userId));
		MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));

		Guard.Against.NullOrEmpty(checkoutItems, nameof(checkoutItems));
		_items.AddRange(checkoutItems);
	}

	public static CheckoutSession Create(Guid userId, Guid merchantId, IEnumerable<CheckoutItem> checkoutItems)
	{
		return new CheckoutSession(userId, merchantId, checkoutItems);
	}

	public void SetAddress(Guid addressId)
	{
		AddressId = addressId;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Id;
		yield return UserId;
		yield return MerchantId;
		if (AddressId.HasValue)
			yield return AddressId.Value;
		if (OrderId.HasValue)
			yield return OrderId.Value;
		foreach (var item in _items)
		{
			yield return item;
		}
	}

	// For EF Core deserialization
	[JsonConstructor]
	private CheckoutSession()
	{
	}
}