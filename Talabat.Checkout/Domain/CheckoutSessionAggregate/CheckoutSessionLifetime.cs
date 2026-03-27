using Talabat.SharedKernal;

namespace Talabat.Checkout.Domain.CheckoutSessionAggregate;

internal class CheckoutSessionLifetime : ValueObject
{
	public CheckoutSessionStatusValues StoredStatus { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime ExpiresAt { get; private set; }

	public static readonly TimeSpan MaxDuration = TimeSpan.FromMinutes(2);

	/// <summary>
	/// Computed status that considers time-based expiration.
	/// If the stored status is Active but the current time exceeds ExpiresAt,
	/// the session is effectively Expired — no background service needed.
	/// </summary>
	public CheckoutSessionStatusValues Status =>
		StoredStatus == CheckoutSessionStatusValues.Active && DateTime.UtcNow >= ExpiresAt
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

	public void Complete() => StoredStatus = CheckoutSessionStatusValues.Completed;
	public void Cancel() => StoredStatus = CheckoutSessionStatusValues.Cancelled;
	public void MarkExpired() => StoredStatus = CheckoutSessionStatusValues.Expired;

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return StoredStatus;
		yield return CreatedAt;
		yield return ExpiresAt;
	}

	// For EF Core
	private CheckoutSessionLifetime() { }
}
