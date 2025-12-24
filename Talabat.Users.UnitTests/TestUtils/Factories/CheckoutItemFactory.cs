using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.UnitTests.TestConstants;

namespace Talabat.Users.UnitTests.TestUtils.Factories;

internal static class CheckoutItemFactory
{
    internal static CheckoutItem Create(
        Guid? productId = null,
        int? quantity = null,
        decimal? basePrice = null)
    {
        var product = productId ?? Constants.Product.Id;
        var qty = quantity ?? Constants.Product.DefaultQuantity;
        var price = basePrice ?? Constants.Product.DefaultPrice;

        var method = typeof(CheckoutItem).GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(Guid), typeof(int), typeof(decimal) },
            null);

        if (method == null)
            throw new InvalidOperationException("Could not find CheckoutItem.Create method");

        return (CheckoutItem)method.Invoke(null, new object[] { product, qty, price })!;
    }

    internal static List<CheckoutItem> CreateList(int count = 2)
    {
        var items = new List<CheckoutItem>();
        
        for (int i = 0; i < count; i++)
        {
            var productId = Guid.NewGuid();
            items.Add(Create(productId));
        }

        return items;
    }
}
