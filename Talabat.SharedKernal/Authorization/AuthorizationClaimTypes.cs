namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Contains claim type constants used in authorization.
/// </summary>
public static class AuthorizationClaimTypes
{
    /// <summary>
    /// Claim type for permissions (e.g., "orders.read", "menu.manage").
    /// </summary>
    public const string Permission = "permission";

    /// <summary>
    /// Claim type for plan tier.
    /// </summary>
    public const string PlanTier = "plan_tier";

    /// <summary>
    /// Claim type for user roles (e.g., "ShopOwner", "Customer", "SystemAdmin").
    /// </summary>
    public const string Role = "role";

    /// <summary>
    /// Claim type for account ID (GUID).
    /// </summary>
    public const string AccountId = "account_id";

    /// <summary>
    /// Claim type for tenant type ("customer" | "shop" | "system").
    /// When tenant_type is "shop", the tenant_id contains the shop ID.
    /// </summary>
    public const string TenantType = "tenant_type";

    /// <summary>
    /// Claim type for tenant ID (Shop/Warehouse GUID when tenant_type is "shop", empty string for customer/system).
    /// </summary>
    public const string TenantId = "tenant_id";

    /// <summary>
    /// Claim type for account display name.
    /// </summary>
    public const string AccountName = "account_name";

    /// <summary>
    /// Claim type for user security stamp hash (for token invalidation).
    /// </summary>
    public const string UserStamp = "user_stamp";

    /// <summary>
    /// Claim type for account security stamp hash (for token invalidation).
    /// </summary>
    public const string AccountStamp = "account_stamp";

    /// <summary>
    /// Claim type for account version (base64 rowversion, used to detect stale tokens).
    /// </summary>
    public const string AccountVersion = "account_version";
}
