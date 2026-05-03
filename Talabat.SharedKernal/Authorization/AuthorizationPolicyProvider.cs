using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Dynamic authorization policy provider that creates policies for permissions and plan features.
/// </summary>
public sealed class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private const string PermissionPolicyPrefix = "Permission:";
    private const string PlanFeaturePolicyPrefix = "PlanFeature:";
    private const string RolePolicyPrefix = "Role:";

    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionPolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[PermissionPolicyPrefix.Length..];
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new AccountVersionRequirement(), new PermissionRequirement(permission))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        if (policyName.StartsWith(PlanFeaturePolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var feature = policyName[PlanFeaturePolicyPrefix.Length..];
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new AccountVersionRequirement(), new PlanFeatureRequirement(feature))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        if (policyName.StartsWith(RolePolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var role = policyName[RolePolicyPrefix.Length..];
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new AccountVersionRequirement(), new RoleRequirement(role))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    /// <summary>
    /// Gets the policy name for a permission.
    /// </summary>
    public static string GetPermissionPolicyName(string permission) => $"{PermissionPolicyPrefix}{permission}";

    /// <summary>
    /// Gets the policy name for a plan feature.
    /// </summary>
    public static string GetPlanFeaturePolicyName(string feature) => $"{PlanFeaturePolicyPrefix}{feature}";

    /// <summary>
    /// Gets the policy name for a role.
    /// </summary>
    public static string GetRolePolicyName(string role) => $"{RolePolicyPrefix}{role}";
}
