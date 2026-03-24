using FluentAssertions;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.UnitTests.TestConstants;
using Talabat.Users.UnitTests.TestUtils.Factories;

namespace Talabat.Users.UnitTests.CustomerAggregate;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldCreateCustomer()
    {
        // Arrange
        var email = Constants.Customer.Email;
        var id = Constants.Customer.Id;

        // Act
        var customer = CustomerFactory.Create(email, id);

        // Assert
        customer.Should().NotBeNull();
        customer.Email.Should().Be(email);
        customer.Id.Should().Be(id);
        customer.Cart.Should().BeNull();
        customer.Addresses.Should().BeEmpty();
        customer.ActiveCheckoutSession.Should().BeNull();
    }

    [Fact]
    public void AddAddress_WithValidAddress_ShouldAddAddressToCustomer()
    {
        // Arrange
        var customer = CustomerFactory.Create();
        var address = Constants.Address.DefaultAddress;

        // Act
        customer.AddAddress(address);

        // Assert
        customer.Addresses.Should().HaveCount(1);
        customer.Addresses.First().Address.Should().Be(address);
    }

    [Fact]
    public void AddAddress_WithMultipleAddresses_ShouldAddAllAddresses()
    {
        // Arrange
        var customer = CustomerFactory.Create();
        var address1 = Constants.Address.DefaultAddress;
        var address2 = Constants.Address.AlternativeAddress;

        // Act
        customer.AddAddress(address1);
        customer.AddAddress(address2);

        // Assert
        customer.Addresses.Should().HaveCount(2);
    }

    [Fact]
    public void SetCartItem_WhenCartIsNull_ShouldCreateCartWithItem()
    {
        // Arrange
        var customer = CustomerFactory.Create();
        var merchantId = Constants.Merchant.Id;
        var productId = Constants.Product.Id;
        var quantity = Constants.Product.DefaultQuantity;

        // Act
        var result = customer.SetCartItem(merchantId, productId, quantity);

        // Assert
        result.IsError.Should().BeFalse();
        customer.Cart.Should().NotBeNull();
        customer.Cart!.MerchantId.Should().Be(merchantId);
        customer.Cart.Items.Should().HaveCount(1);
        customer.Cart.Items.First().ProductId.Should().Be(productId);
        customer.Cart.Items.First().Quantity.Should().Be(quantity);
    }

    [Fact]
    public void SetCartItem_WhenCartExists_ShouldAddItemToCart()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var newProductId = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;

        // Act
        var result = customer.SetCartItem(merchantId, newProductId, quantity);

        // Assert
        result.IsError.Should().BeFalse();
        customer.Cart!.Items.Should().HaveCount(2);
        customer.Cart.Items.Should().Contain(item => item.ProductId == newProductId);
    }

    [Fact]
    public void SetCartItem_WithExistingProduct_ShouldUpdateQuantity()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var productId = Constants.Product.Id;
        var newQuantity = Constants.Product.UpdatedQuantity;

        // Act
        var result = customer.SetCartItem(merchantId, productId, newQuantity);

        // Assert
        result.IsError.Should().BeFalse();
        customer.Cart!.Items.Should().HaveCount(1);
        customer.Cart.Items.First().Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public void SetCartItem_WithDifferentMerchant_ShouldReturnMerchantMismatchError()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var differentMerchantId = Constants.Cart.AlternativeMerchantId;
        var productId = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;

        // Act
        var result = customer.SetCartItem(differentMerchantId, productId, quantity);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.MerchantMismatch);
    }

    [Fact]
    public void SetCartItem_WithActiveCheckoutSession_ShouldReturnError()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        customer.CreateCheckoutSession(checkoutSession);

        var productId = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;

        // Act
        var result = customer.SetCartItem(merchantId, productId, quantity);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.UpdateCartWithActiveCheckoutSession);
    }

    [Fact]
    public void RemoveCartItem_WithExistingItem_ShouldRemoveItem()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var productId = Constants.Product.Id;

        // Act
        var result = customer.RemoveCartItem(productId);

        // Assert
        result.IsError.Should().BeFalse();
        customer.Cart!.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveCartItem_WhenCartIsNull_ShouldReturnCartNotFoundError()
    {
        // Arrange
        var customer = CustomerFactory.Create();
        var productId = Constants.Product.Id;

        // Act
        var result = customer.RemoveCartItem(productId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.CartNotFound);
    }

    [Fact]
    public void ResetCart_WithNoActiveCheckoutSession_ShouldClearCart()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);

        // Act
        var result = customer.ResetCart();

        // Assert
        result.IsError.Should().BeFalse();
        customer.Cart.Should().BeNull();
    }

    [Fact]
    public void ResetCart_WithActiveCheckoutSession_ShouldReturnError()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        customer.CreateCheckoutSession(checkoutSession);

        // Act
        var result = customer.ResetCart();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.UpdateCartWithActiveCheckoutSession);
    }

    [Fact]
    public void CreateCheckoutSession_WithValidCart_ShouldCreateSession()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);

        // Act
        var result = customer.CreateCheckoutSession(checkoutSession);

        // Assert
        result.IsError.Should().BeFalse();
        customer.ActiveCheckoutSession.Should().NotBeNull();
        customer.ActiveCheckoutSession!.UserId.Should().Be(customer.Id);
        customer.ActiveCheckoutSession.MerchantId.Should().Be(merchantId);
    }

    [Fact]
    public void CreateCheckoutSession_WhenCartIsNull_ShouldReturnCartNotFoundError()
    {
        // Arrange
        var customer = CustomerFactory.Create();
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id);

        // Act
        var result = customer.CreateCheckoutSession(checkoutSession);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.CartNotFound);
    }

    [Fact]
    public void CreateCheckoutSession_WithActiveSession_ShouldReturnError()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        customer.CreateCheckoutSession(checkoutSession);

        // Act
        var secondCheckoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        var result = customer.CreateCheckoutSession(secondCheckoutSession);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.ActiveCheckoutSessionExists);
    }

    [Fact]
    public void CancelCheckoutSession_WithActiveSession_ShouldCancelSession()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        customer.CreateCheckoutSession(checkoutSession);

        // Act
        var result = customer.CancelCheckoutSession();

        // Assert
        result.IsError.Should().BeFalse();
        customer.ActiveCheckoutSession.Should().BeNull();
    }

    [Fact]
    public void CancelCheckoutSession_WithNoActiveSession_ShouldReturnError()
    {
        // Arrange
        var customer = CustomerFactory.Create();

        // Act
        var result = customer.CancelCheckoutSession();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.NoActiveCheckoutSession);
    }

    [Fact]
    public void SetAddressForOrder_WithValidAddress_ShouldSetAddress()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        customer.AddAddress(Constants.Address.DefaultAddress);
        var addressId = customer.Addresses.First().Id;

        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        customer.CreateCheckoutSession(checkoutSession);

        // Act
        var result = customer.SetAddressForOrder(addressId);

        // Assert
        result.IsError.Should().BeFalse();
        customer.ActiveCheckoutSession!.AddressId.Should().Be(addressId);
    }

    [Fact]
    public void SetAddressForOrder_WithNoActiveCheckoutSession_ShouldReturnError()
    {
        // Arrange
        var customer = CustomerFactory.CreateWithAddress();
        var addressId = customer.Addresses.First().Id;

        // Act
        var result = customer.SetAddressForOrder(addressId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.NoActiveCheckoutSession);
    }

    [Fact]
    public void SetAddressForOrder_WithNonExistentAddress_ShouldReturnAddressNotFoundError()
    {
        // Arrange
        var merchantId = Constants.Merchant.Id;
        var customer = CustomerFactory.CreateWithCart(merchantId);
        var checkoutSession = CheckoutSessionFactory.Create(customer.Id, merchantId);
        customer.CreateCheckoutSession(checkoutSession);
        var nonExistentAddressId = Guid.NewGuid();

        // Act
        var result = customer.SetAddressForOrder(nonExistentAddressId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(CustomerErrors.AddressNotFound);
    }
}
