using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Checkout.Domain.CheckoutSessionAggregate.Events;
using Talabat.Checkout.Domain.ProductAggregate;
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

		_domainEvents.Add(new CheckoutSessionCreatedEvent(Id, CustomerId));
	}

	public ErrorOr<Success> InitiatePayment(Guid paymentId)
	{
		if (!Lifetime.IsActive)
			return CheckoutSessionErrors.NotActive;

		PaymentId = Guard.Against.Default(paymentId, nameof(paymentId));
		return Result.Success;
	}

	public ErrorOr<Success> Complete()
	{
		var lifetimeResult = Lifetime.Complete();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionCompletedEvent(Id, CustomerId, PaymentId!.Value));

		return Result.Success;
	}

	public ErrorOr<Success> Cancel()
	{
		var lifetimeResult = Lifetime.Cancel();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionCancelledEvent(Id, CustomerId));

		return Result.Success;
	}

	public ErrorOr<Success> Expire()
	{
		var lifetimeResult = Lifetime.MarkExpired();
		if (lifetimeResult.IsError)
			return lifetimeResult.Errors;

		Lifetime = lifetimeResult.Value;

		_domainEvents.Add(new CheckoutSessionExpiredEvent(Id, CustomerId));

		return Result.Success;
	}

	public ErrorOr<Success> Validate(IReadOnlyList<Product> checkoutProducts)
	{
		if (!Lifetime.IsActive)
			return CheckoutSessionErrors.SessionExpired;

		foreach (var item in _items)
		{
			var product = checkoutProducts.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null || product.IsDeleted)
				return CheckoutSessionErrors.ProductNotFound(item.ProductId);

			if (product.BasePrice != item.Price)
				return CheckoutSessionErrors.PriceMismatch(item.ProductId);
		}

		return Result.Success;
	}

	// For EF Core deserialization
	protected CheckoutSession() { }
}
