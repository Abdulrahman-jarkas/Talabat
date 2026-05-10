using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Action filter that enforces plan limits on endpoints marked with EnforcePlanLimitAttribute.
/// </summary>
public sealed class PlanLimitActionFilter : IAsyncActionFilter
{
    private const string ShopIdClaimType = "shop_id";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var enforcePlanLimitAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<EnforcePlanLimitAttribute>()
            .FirstOrDefault();

        if (enforcePlanLimitAttribute is null)
        {
            await next();
            return;
        }

        var shopIdClaim = context.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ShopIdClaimType);

        if (shopIdClaim is null || !Guid.TryParse(shopIdClaim.Value, out var shopId))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Error = "Shop ID claim is missing or invalid."
            });
            return;
        }

        var planLimitService = context.HttpContext.RequestServices.GetService<IPlanLimitService>();
        if (planLimitService is null)
        {
            // No plan limit service registered for this feature, allow the request
            await next();
            return;
        }

        var result = await planLimitService.CheckLimitAsync(
            shopId,
            enforcePlanLimitAttribute.Feature,
            context.HttpContext.RequestAborted);

        if (!result.Allowed)
        {
            context.Result = new ObjectResult(new PlanLimitExceededResponse
            {
                Error = "Plan limit exceeded",
                Feature = result.Feature,
                CurrentUsage = result.CurrentUsage,
                MaxAllowed = result.MaxAllowed,
                IsUnlimited = result.IsUnlimited
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        await next();
    }
}

/// <summary>
/// Response returned when a plan limit is exceeded.
/// </summary>
public sealed class PlanLimitExceededResponse
{
    public string Error { get; init; } = string.Empty;
    public string Feature { get; init; } = string.Empty;
    public int CurrentUsage { get; init; }
    public int? MaxAllowed { get; init; }
    public bool IsUnlimited { get; init; }
}
