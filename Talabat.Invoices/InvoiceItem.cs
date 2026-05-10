using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Invoices;

internal class InvoiceItem : ValueObject
{
	public Guid ProductId { get; init; }
	public int Quantity { get; init; }
	public decimal BasePrice { get; init; }

	private InvoiceItem(Guid productId, int quantity, decimal basePrice)
	{
		productId = Guard.Against.Default(productId, nameof(productId));
		quantity = Guard.Against.NegativeOrZero(quantity, nameof(quantity));
		basePrice = Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));
	}

	public static InvoiceItem Create(Guid productId, int quantity, decimal basePrice)
	{
		return new InvoiceItem(productId, quantity, basePrice);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ProductId;
		yield return Quantity;
		yield return BasePrice;
	}
}
