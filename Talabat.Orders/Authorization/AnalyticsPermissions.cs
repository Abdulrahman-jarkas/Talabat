namespace Talabat.Orders.Authorization;

/// <summary>
/// Permissions for analytics features.
/// </summary>
public static class AnalyticsPermissions
{
    private const string Module = "analytics";

    /// <summary>
    /// Permission to read analytics data.
    /// </summary>
    public const string Read = $"{Module}.read";

    /// <summary>
    /// Permission to export analytics data.
    /// </summary>
    public const string Export = $"{Module}.export";

    /// <summary>
    /// Gets all permissions in this module.
    /// </summary>
    public static IReadOnlyList<string> All => [Read, Export];
}
