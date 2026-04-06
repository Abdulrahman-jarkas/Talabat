using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.OrderAggregate;

internal class OrderItem : ValueObject
{
	public Guid ProductId { get; init; }
	public int Quantity { get; init; }

	private OrderItem(Guid productId, int quantity)
	{
		ProductId = Guard.Against.Default(productId, nameof(productId));
		Quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
	}

	[JsonConstructor]
	private OrderItem() { }

	public static OrderItem Create(Guid productId, int quantity)
	{
		return new OrderItem(productId, quantity);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return Quantity;
	}
}
