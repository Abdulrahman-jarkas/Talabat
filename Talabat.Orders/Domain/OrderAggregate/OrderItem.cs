using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate;

internal class OrderItem : ValueObject
{
	public Guid ProductId { get; init; }
	public int Quantity { get; init; }
	public decimal BasePrice { get; init; }

	private OrderItem(Guid productId, int quantity, decimal basePrice)
	{
		ProductId = Guard.Against.Default(productId, nameof(productId));
		Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
		BasePrice = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
	}

	[JsonConstructor]
	private OrderItem() { }

	public static OrderItem Create(Guid productId, int quantity, decimal basePrice)
	{
		return new OrderItem(productId, quantity, basePrice);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return Quantity;
		yield return BasePrice;
	}
}
