using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.ProductAggregate;

internal class Product : AggregateRoot
{
	public int Quantity { get; private set; }
	public int ReservedQuantity { get; private set; }
	public decimal BasePrice { get; private set; }
	public bool IsDeleted { get; private set; }

	public int AvailableQuantity => Quantity - ReservedQuantity;

	internal Product(Guid id, int quantity, decimal basePrice) : base(id)
	{
		Quantity = Guard.Against.Negative(quantity);
		ReservedQuantity = 0;
		BasePrice = Guard.Against.NegativeOrZero(basePrice);
		IsDeleted = false;
	}

	public ErrorOr<Success> Reserve(int quantity)
	{
		if (quantity <= 0)
			return ProductErrors.InvalidQuantity;

		if (AvailableQuantity < quantity)
			return ProductErrors.InsufficientStock(Id);

		ReservedQuantity += quantity;
		return Result.Success;
	}

	public ErrorOr<Success> Release(int quantity)
	{
		if (quantity <= 0)
			return ProductErrors.InvalidQuantity;

		if (ReservedQuantity < quantity)
			return ProductErrors.InvalidRelease(Id);

		ReservedQuantity -= quantity;
		return Result.Success;
	}

	public ErrorOr<Success> UpdateQuantity(int newQuantity)
	{
		if (newQuantity < 0)
			return ProductErrors.NegativeQuantity;

		if (Quantity == newQuantity)
			return Result.Success;

		Quantity = newQuantity;
		return Result.Success;
	}

	public ErrorOr<Success> UpdatePrice(decimal newPrice)
	{
		if (newPrice <= 0)
			return ProductErrors.InvalidPrice;

		if (BasePrice == newPrice)
			return Result.Success;

		BasePrice = newPrice;
		return Result.Success;
	}

	public ErrorOr<Success> MarkDeleted()
	{
		if (IsDeleted)
			return Result.Success;

		IsDeleted = true;
		return Result.Success;
	}

	// For EF Core
	private Product() { }
}
