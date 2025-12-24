using System.Reflection;
using Talabat.Users.Domain.MerchantAggregate;
using Talabat.Users.UnitTests.TestConstants;

namespace Talabat.Users.UnitTests.TestUtils.Factories;

internal static class MerchantFactory
{
    internal static Merchant Create(
        string? email = null,
        Guid? id = null)
    {
        var merchantEmail = email ?? Constants.Merchant.Email;
        var merchantId = id ?? Constants.Merchant.Id;

        var constructor = typeof(Merchant).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(string), typeof(Guid?) },
            null);

        if (constructor == null)
            throw new InvalidOperationException("Could not find Merchant constructor");

        return (Merchant)constructor.Invoke(new object?[] { merchantEmail, merchantId });
    }
}
