namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Marker attribute to enforce plan limits on an endpoint.
/// Used in conjunction with PlanLimitActionFilter.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class EnforcePlanLimitAttribute : Attribute
{
    /// <summary>
    /// The feature to enforce the limit for (e.g., "products", "orders-per-day").
    /// </summary>
    public string Feature { get; }

    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="feature">The feature name to enforce limits for.</param>
    public EnforcePlanLimitAttribute(string feature)
    {
        Feature = feature;
    }
}
