namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Static configuration for plan features and limits.
/// Features are registered by each module during startup.
/// </summary>
public static class PlanConfiguration
{
    private static readonly Dictionary<PlanTier, Dictionary<string, PlanFeatureConfig>> _planFeatures = new()
    {
        [PlanTier.Free] = [],
        [PlanTier.Basic] = [],
        [PlanTier.Pro] = [],
        [PlanTier.Enterprise] = []
    };

    /// <summary>
    /// Registers a feature configuration for all plan tiers.
    /// </summary>
    /// <param name="feature">The feature name (e.g., "products", "orders-per-day").</param>
    /// <param name="freeConfig">Configuration for Free tier.</param>
    /// <param name="basicConfig">Configuration for Basic tier.</param>
    /// <param name="proConfig">Configuration for Pro tier.</param>
    /// <param name="enterpriseConfig">Configuration for Enterprise tier.</param>
    public static void RegisterFeature(
        string feature,
        PlanFeatureConfig freeConfig,
        PlanFeatureConfig basicConfig,
        PlanFeatureConfig proConfig,
        PlanFeatureConfig enterpriseConfig)
    {
        _planFeatures[PlanTier.Free][feature] = freeConfig;
        _planFeatures[PlanTier.Basic][feature] = basicConfig;
        _planFeatures[PlanTier.Pro][feature] = proConfig;
        _planFeatures[PlanTier.Enterprise][feature] = enterpriseConfig;
    }

    /// <summary>
    /// Gets the feature configuration for a specific plan tier.
    /// </summary>
    /// <param name="tier">The plan tier.</param>
    /// <param name="feature">The feature name.</param>
    /// <returns>The feature configuration, or a disabled config if not found.</returns>
    public static PlanFeatureConfig GetFeatureConfig(PlanTier tier, string feature)
    {
        if (_planFeatures.TryGetValue(tier, out var features) &&
            features.TryGetValue(feature, out var config))
        {
            return config;
        }

        return new PlanFeatureConfig(Enabled: false, Limit: 0);
    }

    /// <summary>
    /// Checks if a feature is enabled for a specific plan tier.
    /// </summary>
    /// <param name="tier">The plan tier.</param>
    /// <param name="feature">The feature name.</param>
    /// <returns>True if the feature is enabled.</returns>
    public static bool IsFeatureEnabled(PlanTier tier, string feature)
    {
        return GetFeatureConfig(tier, feature).Enabled;
    }

    /// <summary>
    /// Gets the limit for a feature on a specific plan tier.
    /// </summary>
    /// <param name="tier">The plan tier.</param>
    /// <param name="feature">The feature name.</param>
    /// <returns>The limit, or null if unlimited.</returns>
    public static int? GetLimit(PlanTier tier, string feature)
    {
        return GetFeatureConfig(tier, feature).Limit;
    }
}
