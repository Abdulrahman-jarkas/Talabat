using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Users;

internal class CheckoutItem : ValueObject
{
	public Guid ProductId { get; init; }
	public int Quantity { get; init; }
	public decimal BasePrice { get; init; }

	private CheckoutItem(Guid productId, int quantity, decimal basePrice)
	{
		productId = Guard.Against.Default(productId, nameof(productId));
		quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
		basePrice = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
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
}
