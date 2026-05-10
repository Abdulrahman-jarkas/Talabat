using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate;

internal class CheckoutItem : Entity
{
	public Guid ProductId { get; private set; }
	public decimal Price { get; private set; }
	public int Quantity { get; private set; }

	private CheckoutItem(Guid productId, decimal price, int quantity, Guid? id = null)
		: base(id ?? Guid.NewGuid())
	{
		ProductId = Guard.Against.Default(productId, nameof(productId));
		Price = Guard.Against.NegativeOrZero(price, nameof(price));
		Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
	}

	public static CheckoutItem Create(Guid productId, decimal price, int quantity)
	{
		return new CheckoutItem(productId, price, quantity);
	}

	// For EF Core deserialization
	private CheckoutItem() { }
}
