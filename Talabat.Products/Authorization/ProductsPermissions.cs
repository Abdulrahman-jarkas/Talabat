namespace Talabat.Products.Authorization;

/// <summary>
/// Permissions for the Products module.
/// </summary>
public static class ProductsPermissions
{
    private const string Module = "products";

    /// <summary>
    /// Permission to create products.
    /// </summary>
    public const string Create = $"{Module}.create";

    /// <summary>
    /// Permission to read/view products.
    /// </summary>
    public const string Read = $"{Module}.read";

    /// <summary>
    /// Permission to update products.
    /// </summary>
    public const string Update = $"{Module}.update";

    /// <summary>
    /// Permission to delete products.
    /// </summary>
    public const string Delete = $"{Module}.delete";

    /// <summary>
    /// Gets all permissions in this module.
    /// </summary>
    public static IReadOnlyList<string> All => [Create, Read, Update, Delete];
}
