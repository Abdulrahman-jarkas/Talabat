using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Orders.Domain.CheckoutSessionAggregate.Events;
using Talabat.SharedKernal;

namespace Talabat.Orders.Domain.CheckoutSessionAggregate;

internal class CheckoutSession : AggregateRoot
{
	private readonly List<CheckoutItem> _items = new();
	public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

	public Guid CustomerId { get; private set; }
	public Guid MerchantId { get; private set; }
	public Guid AddressId { get; private set; }
	public CheckoutSessionLifetime Lifetime { get; private set; }
	public Guid? PaymentId { get; private set; }

	public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

	internal CheckoutSession(
		Guid customerId,
		Guid merchantId,
		IEnumerable<CheckoutItem> items,
		Guid? id = null)
		: base(id ?? Guid.NewGuid())
	{
		CustomerId = Guard.Against.Default(customerId, nameof(customerId));
		MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));

		Guard.Against.NullOrEmpty(items, nameof(items));
		_items.AddRange(items);

		Lifetime = CheckoutSessionLifetime.Create();
		PaymentId = null;

		_domainEvents.Add(new CheckoutSessionCreatedEvent(Id, CustomerId));
	}

	private ErrorOr<Success> ValidateProducts(IReadOnlyList<(Guid ProductId, decimal Price, int AvailableQuantity)> currentProducts)
	{
		var productMap = currentProducts.ToDictionary(p => p.ProductId, p => p);

		foreach (var item in _items)
		{
			if (!productMap.TryGetValue(item.ProductId, out var product))
				return CheckoutSessionErrors.ProductNotFound(item.ProductId);

			if (product.Price != item.Price)
				return CheckoutSessionErrors.PriceMismatch(item.ProductId);

			if (product.AvailableQuantity < item.Quantity)
				return CheckoutSessionErrors.InsufficientStock(item.ProductId);
		}

		return Result.Success;
	}

	public ErrorOr<Success> Checkout(Guid paymentId, Guid addressId, IReadOnlyList<(Guid ProductId, decimal Price, int AvailableQuantity)> currentProducts)
	{
		var lifetimeResult = Lifetime.Checkout();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		var productValidation = ValidateProducts(currentProducts);
		if (productValidation.IsError)
			return productValidation.Errors;

		PaymentId = Guard.Against.Default(paymentId, nameof(paymentId));
		AddressId = Guard.Against.Default(addressId, nameof(addressId));
		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionCheckedOutEvent(
			Id,
			CustomerId,
			_items.Select(i => new CheckoutCheckedOutItem(i.ProductId, i.Quantity)).ToList()));

		return Result.Success;
	}

	public ErrorOr<Success> Complete()
	{
		var lifetimeResult = Lifetime.Complete();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionCompletedEvent(
			Id,
			CustomerId,
			MerchantId,
			PaymentId!.Value,
			AddressId,
			_items.Select(i => new CheckoutCompletedItem(i.ProductId, i.Quantity)).ToList()));

		return Result.Success;
	}

	public ErrorOr<Success> Cancel()
	{
		var lifetimeResult = Lifetime.Cancel();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionCancelledEvent(Id, CustomerId, _items.Select(i => i.ProductId).ToList()));

		return Result.Success;
	}

	public ErrorOr<Success> Close()
	{
		var lifetimeResult = Lifetime.Close();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionClosedEvent(Id, CustomerId, _items.Select(i => i.ProductId).ToList()));

		return Result.Success;
	}

	public ErrorOr<Success> Expire()
	{
		var lifetimeResult = Lifetime.MarkExpired();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionExpiredEvent(Id, CustomerId, _items.Select(i => i.ProductId).ToList()));

		return Result.Success;
	}

	// For EF Core deserialization
	protected CheckoutSession() { }
}
