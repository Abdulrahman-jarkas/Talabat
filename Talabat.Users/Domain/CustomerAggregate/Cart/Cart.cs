using Ardalis.GuardClauses;
using ErrorOr;
using System.Text.Json.Serialization;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Cart;

internal class Cart : ValueObject
{
	// Use init to allow JSON deserialization but prevent mutation after construction
	public IReadOnlyList<CartItem> Items { get; init; } = new List<CartItem>();

	public Guid ShopId { get; init; }

	internal Cart(Guid shopId)
	{
		ShopId = Guard.Against.Default(shopId);
		Items = new List<CartItem>();
	}

	// Private constructor for creating new instances with modified items
	private Cart(Guid shopId, List<CartItem> items)
	{
		ShopId = shopId;
		Items = items;
	}

	public ErrorOr<Cart> SetCartItem(Guid productId, int quantity)
	{
		var newItems = new List<CartItem>(Items);
		var existingItemIndex = newItems.FindIndex(i => i.ProductId == productId);

		if (existingItemIndex >= 0)
		{
			newItems[existingItemIndex] = CartItem.Create(productId, quantity);
		}
		else
		{
			newItems.Add(CartItem.Create(productId, quantity));
		}

		return new Cart(ShopId, newItems);
	}

	public ErrorOr<Cart> RemoveCartItem(Guid productId)
	{
		var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
		if (existingItem is null)
			return CartErrors.CartItemNotFound;

		var newItems = new List<CartItem>(Items);
		newItems.Remove(existingItem);
		return new Cart(ShopId, newItems);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ShopId;
		foreach (var item in Items)
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