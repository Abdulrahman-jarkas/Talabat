using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal class CheckoutItem : Entity
{
	public Guid ProductId { get; private set; }
	public int Quantity { get; private set; }
	public decimal BasePrice { get; private set; }

	private CheckoutItem(Guid productId, int quantity, decimal basePrice, Guid? id = null)
		: base(id ?? Guid.NewGuid())
	{
		ProductId = Guard.Against.Default(productId, nameof(productId));
		Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
		BasePrice = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
	}

	public static CheckoutItem Create(Guid productId, int quantity, decimal basePrice)
	{
		return new CheckoutItem(productId, quantity, basePrice);
	}

	// For EF Core deserialization
	private CheckoutItem() { }
}
