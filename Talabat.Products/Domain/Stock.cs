using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

internal class Stock : ValueObject
{
	public int Quantity { get; private init; }
	public IReadOnlyDictionary<Guid, Reservation> Reservations { get; private init; }
		= new Dictionary<Guid, Reservation>();

	public int EffectiveQuantity => Quantity - Reservations.Values.Sum(r => r.Quantity);
	public bool HasPaidReservations => Reservations.Values.Any(r => r.OrderId is not null);

	private Stock(int quantity, Dictionary<Guid, Reservation> reservations)
	{
		Quantity = quantity;
		Reservations = reservations;
	}

	public static Stock Create(int quantity)
	{
		Guard.Against.Negative(quantity);
		return new Stock(quantity, []);
	}

	public ErrorOr<Stock> AddReservation(Guid checkoutSessionId, Guid userId, int quantity)
	{
		if (quantity <= 0)
			return StockErrors.InvalidQuantity;

		if (Reservations.ContainsKey(checkoutSessionId))
			return this; // Idempotent

		if (EffectiveQuantity < quantity)
			return StockErrors.InsufficientStock;

		var reservation = new Reservation(checkoutSessionId, userId, quantity);
		var updated = new Dictionary<Guid, Reservation>(Reservations)
		{
			[checkoutSessionId] = reservation
		};
		return new Stock(Quantity, updated);
	}

	public ErrorOr<Stock> RemoveReservation(Guid checkoutSessionId)
	{
		if (!Reservations.ContainsKey(checkoutSessionId))
			return StockErrors.ReservationAlreadyRemoved(checkoutSessionId);

		var updated = new Dictionary<Guid, Reservation>(Reservations);
		updated.Remove(checkoutSessionId);
		return new Stock(Quantity, updated);
	}

	public ErrorOr<Stock> SetOrderId(Guid checkoutSessionId, Guid orderId)
	{
		if (!Reservations.TryGetValue(checkoutSessionId, out var existing))
			return StockErrors.ReservationNotFound(checkoutSessionId);

		if (existing.OrderId == orderId)
			return this; // Idempotent

		if (existing.OrderId is not null)
			return StockErrors.OrderAlreadySet(checkoutSessionId);

		var updated = new Dictionary<Guid, Reservation>(Reservations)
		{
			[checkoutSessionId] = existing with { OrderId = orderId }
		};
		return new Stock(Quantity, updated);
	}

	public ErrorOr<Stock> DeductShipped(Guid orderId)
	{
		var entry = Reservations.FirstOrDefault(kvp => kvp.Value.OrderId == orderId);
		if (entry.Value is null)
			return StockErrors.ShipmentReservationNotFound(orderId);

		var updated = new Dictionary<Guid, Reservation>(Reservations);
		updated.Remove(entry.Key);
		return new Stock(Quantity - entry.Value.Quantity, updated);
	}

	public Stock RemoveAllUnpaidReservations()
	{
		var paid = Reservations
			.Where(kvp => kvp.Value.OrderId is not null)
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		if (paid.Count == Reservations.Count)
			return this; // Nothing to remove

		return new Stock(Quantity, paid);
	}

	public ErrorOr<Stock> WithUpdatedQuantity(int newQuantity)
	{
		if (newQuantity < 0)
			return StockErrors.NegativeQuantity;

		if (newQuantity >= Quantity)
			return new Stock(newQuantity, new Dictionary<Guid, Reservation>(Reservations));

		// Reducing quantity — check if effective quantity remains valid
		var totalReserved = Reservations.Values.Sum(r => r.Quantity);
		var newEffective = newQuantity - totalReserved;

		if (newEffective >= 0)
			return new Stock(newQuantity, new Dictionary<Guid, Reservation>(Reservations));

		// Remove unpaid reservations (latest first) until effective >= 0
		var unpaid = Reservations.Values
			.Where(r => r.OrderId is null)
			.OrderByDescending(r => r.CreatedAt)
			.ToList();

		var deficit = -newEffective;
		var toRemove = new HashSet<Guid>();

		foreach (var reservation in unpaid)
		{
			toRemove.Add(reservation.CheckoutSessionId);
			deficit -= reservation.Quantity;

			if (deficit <= 0)
				break;
		}

		if (deficit > 0)
			return StockErrors.CannotReduceQuantity;

		var remaining = Reservations
			.Where(kvp => !toRemove.Contains(kvp.Key))
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		return new Stock(newQuantity, remaining);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Quantity;
		foreach (var kvp in Reservations.OrderBy(kvp => kvp.Key))
			yield return kvp.Value;
	}

	// For EF Core
	private Stock() { }
}
