using FluentAssertions;
using System.Reflection;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.UnitTests.TestConstants;

namespace Talabat.Users.UnitTests.CustomerAggregate;

public class CartTests
{
    private static Cart CreateCart(Guid? merchantId = null)
    {
        var merchant = merchantId ?? Constants.Merchant.Id;

        var constructor = typeof(Cart).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(Guid) },
            null);

        if (constructor == null)
            throw new InvalidOperationException("Could not find Cart constructor");

        return (Cart)constructor.Invoke(new object[] { merchant });
    }

    [Fact]
    public void Create_WithValidMerchantId_ShouldCreateCart()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;

        // Act
        var cart = CreateCart(merchantId);

        // Assert
        cart.Should().NotBeNull();
        cart.MerchantId.Should().Be(merchantId);
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void SetCartItem_WithNewProduct_ShouldAddItemToCart()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Constants.Product.Id;
        var quantity = Constants.Product.DefaultQuantity;

        // Act
        var result = cart.SetCartItem(productId, quantity);

        // Assert
        result.IsError.Should().BeFalse();
        cart.Items.Should().HaveCount(1);
        cart.Items.First().ProductId.Should().Be(productId);
        cart.Items.First().Quantity.Should().Be(quantity);
    }

    [Fact]
    public void SetCartItem_WithExistingProduct_ShouldUpdateQuantity()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Constants.Product.Id;
        var initialQuantity = Constants.Product.DefaultQuantity;
        var updatedQuantity = Constants.Product.UpdatedQuantity;

        cart.SetCartItem(productId, initialQuantity);

        // Act
        var result = cart.SetCartItem(productId, updatedQuantity);

        // Assert
        result.IsError.Should().BeFalse();
        cart.Items.Should().HaveCount(1);
        cart.Items.First().Quantity.Should().Be(updatedQuantity);
    }

    [Fact]
    public void SetCartItem_WithMultipleProducts_ShouldAddAllItems()
    {
        // Arrange
        var cart = CreateCart();
        var product1Id = Constants.Product.Id;
        var product2Id = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;

        // Act
        cart.SetCartItem(product1Id, quantity);
        var result = cart.SetCartItem(product2Id, quantity);

        // Assert
        result.IsError.Should().BeFalse();
        cart.Items.Should().HaveCount(2);
        cart.Items.Should().Contain(item => item.ProductId == product1Id);
        cart.Items.Should().Contain(item => item.ProductId == product2Id);
    }

    [Fact]
    public void RemoveCartItem_WithExistingItem_ShouldRemoveItem()
    {
        // Arrange
        var cart = CreateCart();
        var productId = Constants.Product.Id;
        var quantity = Constants.Product.DefaultQuantity;
        cart.SetCartItem(productId, quantity);

        // Act
        var result = cart.RemoveCartItem(productId);

        // Assert
        result.IsError.Should().BeFalse();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveCartItem_WithNonExistentItem_ShouldReturnCartItemNotFoundError()
    {
        // Arrange
        var cart = CreateCart();
        var nonExistentProductId = Constants.Product.Id;

        // Act
        var result = cart.RemoveCartItem(nonExistentProductId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CartErrors.CartItemNotFound);
    }

    [Fact]
    public void RemoveCartItem_WithMultipleItems_ShouldOnlyRemoveSpecifiedItem()
    {
        // Arrange
        var cart = CreateCart();
        var product1Id = Constants.Product.Id;
        var product2Id = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;

        cart.SetCartItem(product1Id, quantity);
        cart.SetCartItem(product2Id, quantity);

        // Act
        var result = cart.RemoveCartItem(product1Id);

        // Assert
        result.IsError.Should().BeFalse();
        cart.Items.Should().HaveCount(1);
        cart.Items.First().ProductId.Should().Be(product2Id);
    }
}
