using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Authorization;

/// <summary>
/// Registers Orders module plan configuration.
/// </summary>
public static class OrdersPlanConfiguration
{
    /// <summary>
    /// Registers the Orders module features with their plan limits.
    /// </summary>
    public static void Register()
    {
        // Orders per day limit: 5 free, 50 basic, 100 pro, unlimited enterprise
        PlanConfiguration.RegisterFeature(
            Features.OrdersPerDay,
            freeConfig: new PlanFeatureConfig(Enabled: true, Limit: 5),
            basicConfig: new PlanFeatureConfig(Enabled: true, Limit: 50),
            proConfig: new PlanFeatureConfig(Enabled: true, Limit: 100),
            enterpriseConfig: new PlanFeatureConfig(Enabled: true, Limit: null) // unlimited
        );

        // Analytics feature: disabled for free/basic, enabled for pro/enterprise
        PlanConfiguration.RegisterFeature(
            Features.Analytics,
            freeConfig: new PlanFeatureConfig(Enabled: false),
            basicConfig: new PlanFeatureConfig(Enabled: false),
            proConfig: new PlanFeatureConfig(Enabled: true),
            enterpriseConfig: new PlanFeatureConfig(Enabled: true)
        );

        // Advanced reports: disabled for free/basic, enabled for pro/enterprise
        PlanConfiguration.RegisterFeature(
            Features.AdvancedReports,
            freeConfig: new PlanFeatureConfig(Enabled: false),
            basicConfig: new PlanFeatureConfig(Enabled: false),
            proConfig: new PlanFeatureConfig(Enabled: true),
            enterpriseConfig: new PlanFeatureConfig(Enabled: true)
        );
    }
}
