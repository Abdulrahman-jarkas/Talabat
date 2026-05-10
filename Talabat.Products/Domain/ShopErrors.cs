using ErrorOr;

namespace Talabat.Products.Domain;

internal static class ShopErrors
{
    public static Error NotFound(Guid shopId) =>
        Error.NotFound(
            "Shop.NotFound",
            $"Shop '{shopId}' was not found.");

    public static Error AlreadyDeleted(Guid shopId) =>
        Error.Conflict(
            "Shop.AlreadyDeleted",
            $"Shop '{shopId}' is already deleted.");
}
