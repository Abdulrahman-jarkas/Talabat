namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Result of a plan limit check.
/// </summary>
/// <param name="Allowed">Whether the action is allowed within the limit.</param>
/// <param name="CurrentUsage">Current usage count.</param>
/// <param name="MaxAllowed">Maximum allowed usage, null if unlimited.</param>
/// <param name="Feature">The feature being checked.</param>
public record PlanLimitResult(
    bool Allowed,
    int CurrentUsage,
    int? MaxAllowed,
    string Feature)
{
    /// <summary>
    /// Whether the limit is unlimited.
    /// </summary>
    public bool IsUnlimited => MaxAllowed is null;

    /// <summary>
    /// Remaining usage before limit is reached. Returns int.MaxValue if unlimited.
    /// </summary>
    public int Remaining => IsUnlimited ? int.MaxValue : Math.Max(0, MaxAllowed!.Value - CurrentUsage);

    /// <summary>
    /// Creates a successful result for unlimited features.
    /// </summary>
    public static PlanLimitResult Unlimited(string feature, int currentUsage = 0)
        => new(Allowed: true, CurrentUsage: currentUsage, MaxAllowed: null, Feature: feature);

    /// <summary>
    /// Creates a result for a limited feature.
    /// </summary>
    public static PlanLimitResult Limited(string feature, int currentUsage, int maxAllowed)
        => new(Allowed: currentUsage < maxAllowed, CurrentUsage: currentUsage, MaxAllowed: maxAllowed, Feature: feature);
}
