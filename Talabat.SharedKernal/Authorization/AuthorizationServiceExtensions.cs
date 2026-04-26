using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Extension methods for registering authorization services.
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    /// Adds the shared authorization infrastructure including handlers and filters.
    /// </summary>
    public static IServiceCollection AddSharedAuthorization(this IServiceCollection services)
    {
        // Register tenant context for accessing JWT claims
        services.AddScoped<ITenantContext, TenantContext>();

        // Register authorization handlers
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton<IAuthorizationHandler, PlanFeatureHandler>();
        services.AddSingleton<IAuthorizationHandler, RoleHandler>();

        // Register dynamic policy provider
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

        // Register action filter for plan limits
        services.AddScoped<PlanLimitActionFilter>();

        // Add to global MVC filters
        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            options.Filters.Add<PlanLimitActionFilter>();
        });

        return services;
    }

    /// <summary>
    /// Registers a module's plan limit service implementation.
    /// </summary>
    public static IServiceCollection AddPlanLimitService<TService>(this IServiceCollection services)
        where TService : class, IPlanLimitService
    {
        services.AddScoped<IPlanLimitService, TService>();
        return services;
    }
}
