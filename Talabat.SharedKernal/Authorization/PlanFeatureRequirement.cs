using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization requirement for plan feature gate access control.
/// </summary>
public sealed class PlanFeatureRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The feature that requires plan access.
    /// </summary>
    public string Feature { get; }

    /// <summary>
    /// Creates a new plan feature requirement.
    /// </summary>
    /// <param name="feature">The required feature.</param>
    public PlanFeatureRequirement(string feature)
    {
        Feature = feature;
    }
}
