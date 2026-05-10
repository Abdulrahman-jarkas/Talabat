namespace Talabat.Products.Authorization;

public static class ShopsPermissions
{
    private const string Module = "shops";

    public const string Create = $"{Module}.create";
    public const string Read = $"{Module}.read";
    public const string Update = $"{Module}.update";
    public const string Delete = $"{Module}.delete";
    public const string List = $"{Module}.list";

    public static IReadOnlyList<string> All => [Create, Read, Update, Delete, List];

    public static IReadOnlyList<string> System => All;

    public static IReadOnlyList<string> Shop => [Read, Update, List];

    public static IReadOnlyList<string> Customer => [Read, List];
}
