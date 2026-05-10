using Microsoft.AspNetCore.Http;
using Talabat.Products.Data.Repositories;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Authorization;

/// <summary>
/// Plan limit service implementation for the Products module.
/// </summary>
internal sealed class ProductsPlanLimitService : IPlanLimitService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductsPlanLimitService(
        IProductsRepository productsRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _productsRepository = productsRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PlanLimitResult> CheckLimitAsync(Guid shopId, string feature, CancellationToken ct)
    {
        var planTier = GetPlanTierFromClaims();

        return feature switch
        {
            Features.Products => await CheckProductsLimitAsync(shopId, planTier, ct),
            _ => PlanLimitResult.Unlimited(feature) // Unknown features are unlimited by default
        };
    }

    private PlanTier GetPlanTierFromClaims()
    {
        var planTierClaim = _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == AuthorizationClaimTypes.PlanTier);

        if (planTierClaim is null ||
            !Enum.TryParse<PlanTier>(planTierClaim.Value, ignoreCase: true, out var planTier))
        {
            return PlanTier.Free;
        }

        return planTier;
    }

    private async Task<PlanLimitResult> CheckProductsLimitAsync(Guid shopId, PlanTier planTier, CancellationToken ct)
    {
        var limit = PlanConfiguration.GetLimit(planTier, Features.Products);
        var currentCount = await _productsRepository.CountByShopAsync(shopId, ct);

        if (limit is null)
        {
            return PlanLimitResult.Unlimited(Features.Products, currentCount);
        }

        return PlanLimitResult.Limited(Features.Products, currentCount, limit.Value);
    }
}
