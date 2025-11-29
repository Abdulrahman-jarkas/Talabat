using Talabat.OrderProcessing.Domain.Common;

namespace Talabat.OrderProcessing.Domain.OrderAggregate;

public class OrderItem : ValueObject
{
	public int ProductId { get; init; }
	public decimal ProductPrice { get; init; }
	public List<Modifier> Modifiers { get; set; } = new();
	public int Quantity { get; init; }
	public string Note { get; init; } = string.Empty;
	public decimal Vat { get; init; }

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return ProductPrice;
		yield return Quantity;
		yield return Note;
		yield return Vat;
		foreach (var modifier in Modifiers)
		{
			yield return modifier.Id;
		}
	}

	// here the modifiers without vat for now
	public decimal GetTotalPrice()
	{
		decimal modifiersPrice = Modifiers.Sum(g => g.Price);
		decimal vatValue = ProductPrice * (Vat / 100);
		return ((ProductPrice + vatValue) + modifiersPrice) * Quantity;
	}
}