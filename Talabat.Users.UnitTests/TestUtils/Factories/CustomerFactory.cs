using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.UnitTests.TestConstants;

namespace Talabat.Users.UnitTests.TestUtils.Factories;

internal static class CustomerFactory
{
    internal static Customer Create(
        string? email = null,
        Guid? id = null)
    {
        var customerEmail = email ?? Constants.Customer.Email;
        var customerId = id ?? Constants.Customer.Id;

        var constructor = typeof(Customer).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(string), typeof(Guid?) },
            null);

        if (constructor == null)
            throw new InvalidOperationException("Could not find Customer constructor");

        return (Customer)constructor.Invoke(new object?[] { customerEmail, customerId });
    }

    internal static Customer CreateWithCart(
        Guid? shopId = null,
        string? email = null,
        Guid? id = null)
    {
        var customer = Create(email, id);
        var shop = shopId ?? Constants.Shop.Id;
        var productId = Constants.Product.Id;
        var quantity = Constants.Product.DefaultQuantity;

        customer.SetCartItem(shop, productId, quantity);

        return customer;
    }

    internal static Customer CreateWithAddress(
        string? address = null,
        string? email = null,
        Guid? id = null)
    {
        var customer = Create(email, id);
        var addressValue = address ?? Constants.Address.DefaultAddress;
        
        customer.AddAddress(addressValue);

        return customer;
    }
}


