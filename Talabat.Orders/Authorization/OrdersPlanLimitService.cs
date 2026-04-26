using Microsoft.AspNetCore.Http;
using Talabat.Orders.Data.Repositories;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Authorization;

/// <summary>
/// Plan limit service implementation for the Orders module.
/// </summary>
internal sealed class OrdersPlanLimitService : IPlanLimitService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrdersPlanLimitService(
        IOrdersRepository ordersRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _ordersRepository = ordersRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PlanLimitResult> CheckLimitAsync(Guid shopId, string feature, CancellationToken ct)
    {
        var planTier = GetPlanTierFromClaims();

        return feature switch
        {
            Features.OrdersPerDay => await CheckOrdersPerDayLimitAsync(shopId, planTier, ct),
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

    private async Task<PlanLimitResult> CheckOrdersPerDayLimitAsync(Guid shopId, PlanTier planTier, CancellationToken ct)
    {
        var limit = PlanConfiguration.GetLimit(planTier, Features.OrdersPerDay);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentCount = await _ordersRepository.CountOrdersTodayByShopAsync(shopId, today, ct);

        if (limit is null)
        {
            return PlanLimitResult.Unlimited(Features.OrdersPerDay, currentCount);
        }

        return PlanLimitResult.Limited(Features.OrdersPerDay, currentCount, limit.Value);
    }
}
