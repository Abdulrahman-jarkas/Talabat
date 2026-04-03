using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate;

internal class CheckoutSessionLifetime : ValueObject
{
	public CheckoutSessionStatusValues StoredStatus { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime ExpiresAt { get; private set; }

	public static readonly TimeSpan MaxDuration = TimeSpan.FromMinutes(2);

	private bool IsLifetimeExceeded => StoredStatus == CheckoutSessionStatusValues.Active && DateTime.UtcNow >= ExpiresAt;

	public CheckoutSessionStatusValues Status => IsLifetimeExceeded
		? CheckoutSessionStatusValues.Expired
		: StoredStatus;

	public bool IsActive => Status == CheckoutSessionStatusValues.Active;
	public bool IsExpired => Status == CheckoutSessionStatusValues.Expired;

	private CheckoutSessionLifetime(CheckoutSessionStatusValues status, DateTime createdAt, DateTime expiresAt)
	{
		StoredStatus = status;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}

	public static CheckoutSessionLifetime Create()
	{
		var now = DateTime.UtcNow;
		return new CheckoutSessionLifetime(CheckoutSessionStatusValues.Active, now, now.Add(MaxDuration));
	}

	public ErrorOr<CheckoutSessionLifetime> Complete()
	{
		if (!IsActive)
			return CheckoutSessionErrors.NotActive;

		return new CheckoutSessionLifetime(CheckoutSessionStatusValues.Completed, CreatedAt, ExpiresAt);
	}

	public ErrorOr<CheckoutSessionLifetime> Cancel()
	{
		if (!IsActive)
			return CheckoutSessionErrors.NotActive;

		return new CheckoutSessionLifetime(CheckoutSessionStatusValues.Cancelled, CreatedAt, ExpiresAt);
	}

	public ErrorOr<CheckoutSessionLifetime> MarkExpired()
	{
		if (StoredStatus != CheckoutSessionStatusValues.Active)
			return CheckoutSessionErrors.NotActive;

		return new CheckoutSessionLifetime(CheckoutSessionStatusValues.Expired, CreatedAt, ExpiresAt);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return StoredStatus;
		yield return CreatedAt;
		yield return ExpiresAt;
	}

	// For EF Core
	private CheckoutSessionLifetime() { }
}
