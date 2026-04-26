namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Contains well-known role names used in authorization.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Customer role - can add/remove cart items, place orders.
    /// </summary>
    public const string Customer = "Customer";

    /// <summary>
    /// Shop owner role - can manage products, view analytics.
    /// </summary>
    public const string ShopOwner = "ShopOwner";

    /// <summary>
    /// Admin role - full access.
    /// </summary>
    public const string Admin = "Admin";
}
