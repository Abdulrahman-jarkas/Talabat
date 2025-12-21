using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Users;

internal class Cart : Entity
{
	private readonly List<CartItem> _items = new();
	public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; }

	internal Cart(Guid merchantId)
	{
		MerchantId = Guard.Against.Default(merchantId);
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
}