using FluentAssertions;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
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
	public void ResetCart_ShouldClearCart()
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
}
