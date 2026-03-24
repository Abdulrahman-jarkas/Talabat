using Ardalis.GuardClauses;
using System.Text.Json.Serialization;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal class CheckoutItem : ValueObject
{
	public Guid ProductId { get; init; }
	public int Quantity { get; init; }
	public decimal BasePrice { get; init; }

	private CheckoutItem(Guid productId, int quantity, decimal basePrice)
	{
		ProductId = Guard.Against.Default(productId, nameof(productId));
		Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
		BasePrice = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
	}

	public static CheckoutItem Create(Guid productId, int quantity, decimal basePrice)
	{
		return new CheckoutItem(productId, quantity, basePrice);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return Quantity;
		yield return BasePrice;
	}

	[JsonConstructor]
	private CheckoutItem() { }
}
