using Talabat.OrderProcessing.Domain.Common;

namespace Talabat.OrderProcessing.Domain.OrderAggregate;

public class OrderItem : ValueObject
{
	public int ProductId { get; init; }
	public decimal ProductPrice { get; init; }
	public List<Modifier> Modifiers { get; set; } = new();
	public int Quantity { get; init; }
	public string Note { get; init; } = string.Empty;

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return ProductPrice;
		yield return Quantity;
		yield return Note;
		foreach (var modifier in Modifiers)
		{
			yield return modifier.Id;
		}
	}

	public decimal GetTotalPrice()
	{
		decimal modifiersPrice = Modifiers.Sum(g => g.Price);

		return (ProductPrice + modifiersPrice) * Quantity;
	}
}