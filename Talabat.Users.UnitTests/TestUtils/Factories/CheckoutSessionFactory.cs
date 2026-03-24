using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.UnitTests.TestConstants;

namespace Talabat.Users.UnitTests.TestUtils.Factories;

internal static class CheckoutSessionFactory
{
    internal static CheckoutSession Create(
        Guid? userId = null,
        Guid? merchantId = null,
        List<CheckoutItem>? checkoutItems = null)
    {
        var user = userId ?? Constants.Customer.Id;
        var merchant = merchantId ?? Constants.Merchant.Id;
        var items = checkoutItems ?? CheckoutItemFactory.CreateList();

        var method = typeof(CheckoutSession).GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(Guid), typeof(Guid), typeof(IEnumerable<CheckoutItem>) },
            null);

        if (method == null)
            throw new InvalidOperationException("Could not find CheckoutSession.Create method");

        return (CheckoutSession)method.Invoke(null, new object[] { user, merchant, items })!;
    }
}
