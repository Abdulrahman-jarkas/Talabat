namespace Talabat.Users.Authorization;

/// <summary>
/// Permissions for the Users module.
/// </summary>
public static class UsersPermissions
{
    private const string Module = "users";

    /// <summary>
    /// Permission to read user profiles.
    /// </summary>
    public const string Read = $"{Module}.read";

    /// <summary>
    /// Permission to update user profiles.
    /// </summary>
    public const string Update = $"{Module}.update";

    /// <summary>
    /// Gets all permissions in this module.
    /// </summary>
    public static IReadOnlyList<string> All => [Read, Update];
}

/// <summary>
/// Permissions for the Cart module.
/// Note: Cart operations use role-based authorization (Customer role required).
/// </summary>
public static class CartPermissions
{
    private const string Module = "cart";

    /// <summary>
    /// Permission to read cart.
    /// </summary>
    public const string Read = $"{Module}.read";

    /// <summary>
    /// Permission to add items to cart.
    /// </summary>
    public const string AddItem = $"{Module}.add-item";

    /// <summary>
    /// Permission to remove items from cart.
    /// </summary>
    public const string RemoveItem = $"{Module}.remove-item";

    /// <summary>
    /// Permission to clear cart.
    /// </summary>
    public const string Clear = $"{Module}.clear";

    /// <summary>
    /// Gets all permissions in this module.
    /// </summary>
    public static IReadOnlyList<string> All => [Read, AddItem, RemoveItem, Clear];
}
