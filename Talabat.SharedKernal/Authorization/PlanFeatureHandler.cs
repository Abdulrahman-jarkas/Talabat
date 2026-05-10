using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization handler that checks if the user's plan tier has access to a feature.
/// </summary>
public sealed class PlanFeatureHandler : AuthorizationHandler<PlanFeatureRequirement>
{
    private const string PlanTierClaimType = "plan_tier";

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PlanFeatureRequirement requirement)
    {
        var planTierClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == PlanTierClaimType);

        if (planTierClaim is null)
        {
            return Task.CompletedTask;
        }

        if (!Enum.TryParse<PlanTier>(planTierClaim.Value, ignoreCase: true, out var planTier))
        {
            return Task.CompletedTask;
        }

        if (PlanConfiguration.IsFeatureEnabled(planTier, requirement.Feature))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
