using Ardalis.GuardClauses;
using System.Text.Json.Serialization;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Cart;

internal class CartItem : ValueObject
{
	public Guid ProductId { get; init; }
	public int Quantity { get; init; }

	private CartItem(Guid customerId, int quantity)
	{
		ProductId = Guard.Against.Default(customerId, nameof(customerId));
		Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
	}

	public static CartItem Create(Guid productId, int quantity)
	{
		return new CartItem(productId, quantity);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return Quantity;
	}

	// EF core
	[JsonConstructor]
	private CartItem() { }
}