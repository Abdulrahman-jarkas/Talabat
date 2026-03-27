using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

internal class Stock : ValueObject
{
	public int AvailableStock { get; private set; }
	public int ReservedStock { get; private set; }

	public int EffectiveStock => AvailableStock - ReservedStock;

	private Stock(int availableStock, int reservedStock)
	{
		AvailableStock = availableStock;
		ReservedStock = reservedStock;
	}

	public static Stock Create(int availableStock)
	{
		return new Stock(availableStock, 0);
	}

	public ErrorOr<Success> Reserve(int quantity)
	{
		if (quantity <= 0)
			return Error.Validation("Stock.InvalidQuantity", "Quantity must be positive.");

		if (EffectiveStock < quantity)
			return Error.Conflict(
				"Stock.InsufficientStock",
				$"Insufficient stock. Available: {EffectiveStock}, Requested: {quantity}.");

		ReservedStock += quantity;
		return Result.Success;
	}

	public ErrorOr<Success> Release(int quantity)
	{
		if (quantity <= 0)
			return Error.Validation("Stock.InvalidQuantity", "Quantity must be positive.");

		if (ReservedStock < quantity)
			return Error.Conflict(
				"Stock.InvalidRelease",
				$"Cannot release {quantity} items. Only {ReservedStock} are reserved.");

		ReservedStock -= quantity;
		return Result.Success;
	}

	public ErrorOr<Success> Deduct(int quantity)
	{
		if (quantity <= 0)
			return Error.Validation("Stock.InvalidQuantity", "Quantity must be positive.");

		if (ReservedStock < quantity)
			return Error.Conflict(
				"Stock.InvalidDeduct",
				$"Cannot deduct {quantity} items. Only {ReservedStock} are reserved.");

		AvailableStock -= quantity;
		ReservedStock -= quantity;
		return Result.Success;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return AvailableStock;
		yield return ReservedStock;
	}

	// For EF Core
	private Stock() { }
}
