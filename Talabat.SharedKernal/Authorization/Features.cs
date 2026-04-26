namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Contains well-known feature names used for plan limits and feature gates.
/// </summary>
public static class Features
{
    /// <summary>
    /// Products feature - limit on number of products.
    /// </summary>
    public const string Products = "products";

    /// <summary>
    /// Orders per day feature - limit on daily orders.
    /// </summary>
    public const string OrdersPerDay = "orders-per-day";

    /// <summary>
    /// Analytics feature - requires Pro plan or higher.
    /// </summary>
    public const string Analytics = "analytics";

    /// <summary>
    /// Advanced reports feature - requires Pro plan or higher.
    /// </summary>
    public const string AdvancedReports = "advanced-reports";
}
