using Ardalis.GuardClauses;
using ErrorOr;
using System.Text.Json.Serialization;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Cart;

internal class Cart : ValueObject
{
	private List<CartItem> _items = new();
	public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; init; }

	internal Cart(Guid merchantId)
	{
		MerchantId = Guard.Against.Default(merchantId);
		_items = new List<CartItem>();
	}

	public ErrorOr<Updated> SetCartItem(Guid productId, int quantity)
	{
		var existingItemIndex = _items.FindIndex(i => i.ProductId == productId);

		if (existingItemIndex >= 0)
		{
			_items[existingItemIndex] = CartItem.Create(productId, quantity);
		}
		else
		{
			_items.Add(CartItem.Create(productId, quantity));
		}

		return Result.Updated;
	}

	public ErrorOr<Updated> RemoveCartItem(Guid productId)
	{
		var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
		if (existingItem is null)
			return CartErrors.CartItemNotFound;

		_items.Remove(existingItem);
		return Result.Updated;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return MerchantId;
		foreach (var item in _items)
		{
			yield return item;
		}
	}

	// For EF Core deserialization
	[JsonConstructor]
	private Cart()
	{
	}
}