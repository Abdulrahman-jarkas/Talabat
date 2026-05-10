using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Authorization;

/// <summary>
/// Registers Products module plan configuration.
/// </summary>
public static class ProductsPlanConfiguration
{
    /// <summary>
    /// Registers the Products module features with their plan limits.
    /// </summary>
    public static void Register()
    {
        // Products limit: 10 free, 30 basic, 50 pro, unlimited enterprise
        PlanConfiguration.RegisterFeature(
            Features.Products,
            freeConfig: new PlanFeatureConfig(Enabled: true, Limit: 10),
            basicConfig: new PlanFeatureConfig(Enabled: true, Limit: 30),
            proConfig: new PlanFeatureConfig(Enabled: true, Limit: 50),
            enterpriseConfig: new PlanFeatureConfig(Enabled: true, Limit: null) // unlimited
        );
    }
}
