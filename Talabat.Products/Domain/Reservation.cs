using System.Text.Json.Serialization;
using Ardalis.GuardClauses;

namespace Talabat.Products.Domain;

internal record Reservation
{
	public Guid CheckoutSessionId { get; init; }
	public Guid UserId { get; init; }
	public int Quantity { get; init; }
	public Guid? OrderId { get; init; }
	public DateTime CreatedAt { get; init; }

	internal Reservation(
		Guid checkoutSessionId,
		Guid userId,
		int quantity,
		Guid? orderId = null)
	{
		CheckoutSessionId = Guard.Against.Default(checkoutSessionId);
		UserId = Guard.Against.Default(userId);
		Quantity = Guard.Against.NegativeOrZero(quantity);
		OrderId = orderId;
		CreatedAt = DateTime.UtcNow;
	}

	[JsonConstructor]
	private Reservation() { }
}
