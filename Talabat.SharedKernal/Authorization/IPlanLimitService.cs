namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Service interface for checking plan limits/quotas.
/// Each module implements this interface for its specific features.
/// </summary>
public interface IPlanLimitService
{
    /// <summary>
    /// Checks if an action is allowed within the plan limits for a shop.
    /// </summary>
    /// <param name="shopId">The shop identifier.</param>
    /// <param name="feature">The feature to check (e.g., "products", "orders-per-day").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The plan limit check result.</returns>
    Task<PlanLimitResult> CheckLimitAsync(Guid shopId, string feature, CancellationToken ct);
}
