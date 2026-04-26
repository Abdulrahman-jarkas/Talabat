namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Configuration for a feature within a plan tier.
/// </summary>
/// <param name="Enabled">Whether the feature is available on this plan.</param>
/// <param name="Limit">Maximum usage allowed. Null means unlimited.</param>
public record PlanFeatureConfig(bool Enabled, int? Limit = null)
{
    /// <summary>
    /// Whether the limit is unlimited.
    /// </summary>
    public bool IsUnlimited => Limit is null;
}
