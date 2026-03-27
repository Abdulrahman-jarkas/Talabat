using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Checkout.Application.Services;
using Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;
using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate;

internal class CheckoutSession : AggregateRoot
{
	private readonly List<CheckoutItem> _items = new();
	public IReadOnlyCollection<CheckoutItem> Items => _items.AsReadOnly();

	public Guid CustomerId { get; private set; }
	public Guid MerchantId { get; private set; }
	public Guid AddressId { get; private set; }
	public CheckoutSessionLifetime Lifetime { get; private set; }
	public Guid? PaymentId { get; private set; }
	public Guid? OrderId { get; private set; }

	public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

	internal CheckoutSession(
		Guid customerId,
		Guid merchantId,
		Guid addressId,
		IEnumerable<CheckoutItem> items,
		Guid? id = null)
		: base(id ?? Guid.NewGuid())
	{
		CustomerId = Guard.Against.Default(customerId, nameof(customerId));
		MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
		AddressId = Guard.Against.Default(addressId, nameof(addressId));

		Guard.Against.NullOrEmpty(items, nameof(items));
		_items.AddRange(items);

		Lifetime = CheckoutSessionLifetime.Create();
		PaymentId = null;
		OrderId = null;

		_domainEvents.Add(new CheckoutSessionCreatedEvent(Id, CustomerId));
	}

	public ErrorOr<Success> Complete(Guid paymentId, Guid orderId)
	{
		if (!Lifetime.IsActive)
			return CheckoutSessionErrors.NotActive;

		Guard.Against.Default(paymentId, nameof(paymentId));
		Guard.Against.Default(orderId, nameof(orderId));

		PaymentId = paymentId;
		OrderId = orderId;
		Lifetime.Complete();

		_domainEvents.Add(new CheckoutSessionCompletedEvent(Id, CustomerId, paymentId, orderId));

		return Result.Success;
	}

	public ErrorOr<Success> Cancel()
	{
		if (!Lifetime.IsActive)
			return CheckoutSessionErrors.NotActive;

		Lifetime.Cancel();

		_domainEvents.Add(new CheckoutSessionCancelledEvent(Id, CustomerId));

		return Result.Success;
	}

	public ErrorOr<Success> Expire()
	{
		if (Lifetime.StoredStatus != CheckoutSessionStatusValues.Active)
			return CheckoutSessionErrors.NotActive;

		Lifetime.MarkExpired();

		_domainEvents.Add(new CheckoutSessionExpiredEvent(Id, CustomerId));

		return Result.Success;
	}

	/// <summary>
	/// Checks if the session has exceeded its lifetime and transitions to Expired if so.
	/// Returns true if the session was expired by this call.
	/// </summary>
	public bool TryExpireIfLifetimeExceeded()
	{
		if (Lifetime.StoredStatus != CheckoutSessionStatusValues.Active)
			return false;

		if (DateTime.UtcNow < Lifetime.ExpiresAt)
			return false;

		Lifetime.MarkExpired();
		_domainEvents.Add(new CheckoutSessionExpiredEvent(Id, CustomerId));
		return true;
	}

	public async Task<ErrorOr<Success>> Validate(IProductService productService, CancellationToken cancellationToken = default)
	{
		if (!Lifetime.IsActive)
			return CheckoutSessionErrors.SessionExpired;

		var productIds = _items.Select(i => i.ProductId).ToList();

		var products = await productService.GetProductsForValidationAsync(productIds, cancellationToken);

		if (products is null || products.Count == 0)
			return CheckoutSessionErrors.ProductNotFound(productIds.First());

		foreach (var item in _items)
		{
			var product = products.FirstOrDefault(p => p.ProductId == item.ProductId);

			if (product is null)
				return CheckoutSessionErrors.ProductNotFound(item.ProductId);

			if (product.BasePrice != item.Price)
				return CheckoutSessionErrors.PriceMismatch(item.ProductId);
		}

		return Result.Success;
	}

	// For EF Core deserialization
	protected CheckoutSession() { }
}
