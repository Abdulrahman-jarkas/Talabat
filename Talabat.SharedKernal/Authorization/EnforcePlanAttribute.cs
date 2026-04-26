namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Attribute to require a specific plan tier for a feature.
/// Used in conjunction with PlanFeatureRequirement and PlanFeatureHandler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class EnforcePlanAttribute : Attribute
{
    /// <summary>
    /// The feature that requires a specific plan tier (e.g., "analytics", "advanced-reports").
    /// </summary>
    public string Feature { get; }

    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="feature">The feature name that requires plan access.</param>
    public EnforcePlanAttribute(string feature)
    {
        Feature = feature;
    }
}
