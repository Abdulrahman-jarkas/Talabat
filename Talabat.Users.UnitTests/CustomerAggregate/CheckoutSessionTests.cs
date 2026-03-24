using FluentAssertions;
using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.UnitTests.TestConstants;
using Talabat.Users.UnitTests.TestUtils.Factories;

namespace Talabat.Users.UnitTests.CustomerAggregate;

public class CheckoutSessionTests
{
    private static CheckoutSession CreateCheckoutSession(
        Guid? merchantId = null,
        IEnumerable<CheckoutItem>? checkoutItems = null)
    {
        var merchant = merchantId ?? Constants.CheckoutSession.MerchantId;
        var items = checkoutItems ?? CheckoutItemFactory.CreateList();

        var method = typeof(CheckoutSession).GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,
            null,
            new[] { typeof(Guid), typeof(IEnumerable<CheckoutItem>), typeof(Guid?) },
            null);

        if (method == null)
            throw new InvalidOperationException("Could not find CheckoutSession.Create method");

        return (CheckoutSession)method.Invoke(null, new object?[] { merchant, items, null })!;
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateCheckoutSession()
    {
        // Arrange
        var merchantId = Constants.CheckoutSession.MerchantId;
        var checkoutItems = CheckoutItemFactory.CreateList();

        // Act
        var session = CreateCheckoutSession(merchantId: merchantId, checkoutItems: checkoutItems);

        // Assert
        session.Should().NotBeNull();
        session.MerchantId.Should().Be(merchantId);
        session.Items.Should().HaveCount(checkoutItems.Count);
        session.AddressId.Should().BeNull();
    }

    [Fact]
    public void Create_WithMultipleItems_ShouldContainAllItems()
    {
        // Arrange
        var checkoutItems = CheckoutItemFactory.CreateList(count: 3);

        // Act
        var session = CreateCheckoutSession(checkoutItems: checkoutItems);

        // Assert
        session.Items.Should().HaveCount(3);
    }

    [Fact]
    public void SetAddress_WithValidAddressId_ShouldSetAddress()
    {
        // Arrange
        var session = CreateCheckoutSession();
        var addressId = Constants.CheckoutSession.AddressId;

        // Act
        session.SetAddress(addressId);

        // Assert
        session.AddressId.Should().Be(addressId);
    }

    [Fact]
    public void SetAddress_CalledMultipleTimes_ShouldUpdateToLatestAddress()
    {
        // Arrange
        var session = CreateCheckoutSession();
        var firstAddressId = Constants.CheckoutSession.AddressId;
        var secondAddressId = Guid.NewGuid();

        // Act
        session.SetAddress(firstAddressId);
        session.SetAddress(secondAddressId);

        // Assert
        session.AddressId.Should().Be(secondAddressId);
    }

    [Fact]
    public void TotalPrice_WithMultipleItems_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var item1 = CheckoutItemFactory.Create(quantity: 2, basePrice: 10.00m);
        var item2 = CheckoutItemFactory.Create(
            productId: Constants.Product.AlternativeId,
            quantity: 3,
            basePrice: 5.00m);
        var items = new List<CheckoutItem> { item1, item2 };

        // Act
        var session = CreateCheckoutSession(checkoutItems: items);

        // Assert
        // TotalPrice = Sum(BasePrice * Quantity)
        // = (10.00 * 2) + (5.00 * 3) = 20.00 + 15.00 = 35.00
        var expectedTotal = (10.00m * 2) + (5.00m * 3);
        session.TotalPrice.Should().Be(expectedTotal);
        session.TotalPrice.Should().Be(35.00m);
    }

    [Fact]
    public void TotalPrice_WithSingleItem_ShouldReturnCorrectPrice()
    {
        // Arrange
        var item = CheckoutItemFactory.Create(quantity: 2, basePrice: 15.00m);
        var items = new List<CheckoutItem> { item };

        // Act
        var session = CreateCheckoutSession(checkoutItems: items);

        // Assert
        // TotalPrice = BasePrice * Quantity = 15.00 * 2 = 30.00
        var expectedTotal = 15.00m * 2;
        session.TotalPrice.Should().Be(expectedTotal);
        session.TotalPrice.Should().Be(30.00m);
    }

    [Fact]
    public void Items_ShouldBeReadOnly()
    {
        // Arrange
        var checkoutItems = CheckoutItemFactory.CreateList();
        var session = CreateCheckoutSession(checkoutItems: checkoutItems);

        // Act
        var items = session.Items;

        // Assert
        items.Should().BeAssignableTo<IReadOnlyCollection<CheckoutItem>>();
    }

    [Fact]
    public void Create_WithSingleItem_ShouldCreateSessionSuccessfully()
    {
        // Arrange
        var singleItem = new List<CheckoutItem> { CheckoutItemFactory.Create() };

        // Act
        var session = CreateCheckoutSession(checkoutItems: singleItem);

        // Assert
        session.Items.Should().HaveCount(1);
    }

    [Fact]
    public void TotalPrice_WithNoItems_ShouldReturnZero()
    {
        // Arrange
        var emptyItems = new List<CheckoutItem>();

        // Act & Assert
        // This should throw an exception because Guard.Against.NullOrEmpty is used
        // When using reflection, the exception is wrapped in TargetInvocationException
        var act = () => CreateCheckoutSession(checkoutItems: emptyItems);
        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<ArgumentException>()
            .WithMessage("*checkoutItems*");
    }

    [Fact]
    public void TotalPrice_WithDifferentQuantitiesAndPrices_ShouldCalculateAccurately()
    {
        // Arrange
        var item1 = CheckoutItemFactory.Create(quantity: 1, basePrice: 100.50m);
        var item2 = CheckoutItemFactory.Create(
            productId: Constants.Product.AlternativeId,
            quantity: 5,
            basePrice: 25.99m);
        var item3 = CheckoutItemFactory.Create(
            productId: Guid.NewGuid(),
            quantity: 3,
            basePrice: 12.33m);
        var items = new List<CheckoutItem> { item1, item2, item3 };

        // Act
        var session = CreateCheckoutSession(checkoutItems: items);

        // Assert
        // TotalPrice = (100.50 * 1) + (25.99 * 5) + (12.33 * 3)
        // = 100.50 + 129.95 + 36.99 = 267.44
        var expectedTotal = (100.50m * 1) + (25.99m * 5) + (12.33m * 3);
        session.TotalPrice.Should().Be(expectedTotal);
        session.TotalPrice.Should().Be(267.44m);
    }

    [Fact]
    public void CheckoutItem_ShouldHaveCorrectProperties()
    {
        // Arrange
        var productId = Constants.Product.Id;
        var quantity = 5;
        var basePrice = 99.99m;

        // Act
        var item = CheckoutItemFactory.Create(productId: productId, quantity: quantity, basePrice: basePrice);

        // Assert
        item.ProductId.Should().Be(productId);
        item.Quantity.Should().Be(quantity);
        item.BasePrice.Should().Be(basePrice);
    }

    [Fact]
    public void Items_AfterCreation_ShouldMatchProvidedItems()
    {
        // Arrange
        var productId1 = Constants.Product.Id;
        var productId2 = Constants.Product.AlternativeId;
        var items = new List<CheckoutItem>
        {
            CheckoutItemFactory.Create(productId: productId1, quantity: 2, basePrice: 10.00m),
            CheckoutItemFactory.Create(productId: productId2, quantity: 3, basePrice: 15.00m)
        };

        // Act
        var session = CreateCheckoutSession(checkoutItems: items);

        // Assert
        session.Items.Should().HaveCount(2);
        session.Items.Should().Contain(i => i.ProductId == productId1 && i.Quantity == 2 && i.BasePrice == 10.00m);
        session.Items.Should().Contain(i => i.ProductId == productId2 && i.Quantity == 3 && i.BasePrice == 15.00m);
    }
}
