namespace Talabat.Orders.Authorization;

/// <summary>
/// Permissions for the Orders module.
/// </summary>
public static class OrdersPermissions
{
    private const string Module = "orders";

    /// <summary>
    /// Permission to create orders.
    /// </summary>
    public const string Create = $"{Module}.create";

    /// <summary>
    /// Permission to read/view orders.
    /// </summary>
    public const string Read = $"{Module}.read";

    /// <summary>
    /// Permission to update orders.
    /// </summary>
    public const string Update = $"{Module}.update";

    /// <summary>
    /// Permission to cancel orders.
    /// </summary>
    public const string Cancel = $"{Module}.cancel";

    /// <summary>
    /// Permission to ship orders.
    /// </summary>
    public const string Ship = $"{Module}.ship";

    /// <summary>
    /// Permission to deliver orders.
    /// </summary>
    public const string Deliver = $"{Module}.deliver";

    /// <summary>
    /// Gets all permissions in this module.
    /// </summary>
    public static IReadOnlyList<string> All => [Create, Read, Update, Cancel, Ship, Deliver];
}

/// <summary>
/// Permissions for checkout sessions.
/// </summary>
public static class CheckoutSessionPermissions
{
    private const string Module = "checkout";

    /// <summary>
    /// Permission to create checkout sessions.
    /// </summary>
    public const string Create = $"{Module}.create";

    /// <summary>
    /// Permission to read checkout sessions.
    /// </summary>
    public const string Read = $"{Module}.read";

    /// <summary>
    /// Permission to cancel checkout sessions.
    /// </summary>
    public const string Cancel = $"{Module}.cancel";

    /// <summary>
    /// Permission to complete checkout.
    /// </summary>
    public const string Checkout = $"{Module}.checkout";

    /// <summary>
    /// Gets all permissions in this module.
    /// </summary>
    public static IReadOnlyList<string> All => [Create, Read, Cancel, Checkout];
}
