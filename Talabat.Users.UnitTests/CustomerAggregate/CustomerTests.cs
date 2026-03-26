using FluentAssertions;
using NSubstitute;
using Talabat.Products.Contracts;
using Talabat.Users.Application.Services;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
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
	public async Task SetCartItem_WithActiveCheckoutSession_ShouldReturnError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		await customer.CreateCheckoutSession(productService);

		var alternativeProductId = Constants.Product.AlternativeId;
		var quantity = Constants.Product.DefaultQuantity;

		// Act
		var result = customer.SetCartItem(merchantId, alternativeProductId, quantity);

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
	public async Task ResetCart_WithActiveCheckoutSession_ShouldReturnError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		await customer.CreateCheckoutSession(productService);

		// Act
		var result = customer.ResetCart();

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Should().Be(CustomerErrors.UpdateCartWithActiveCheckoutSession);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithValidCart_ShouldCreateSession()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		// Act
		var result = await customer.CreateCheckoutSession(productService);

		// Assert
		result.IsError.Should().BeFalse();
		customer.ActiveCheckoutSession.Should().NotBeNull();
		customer.ActiveCheckoutSession!.MerchantId.Should().Be(merchantId);
	}

	[Fact]
	public async Task CreateCheckoutSession_WhenCartIsNull_ShouldReturnCartNotFoundError()
	{
		// Arrange
		var customer = CustomerFactory.Create();
		var productService = Substitute.For<IProductService>();

		// Act
		var result = await customer.CreateCheckoutSession(productService);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Should().Be(CustomerErrors.CartNotFound);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithActiveSession_ShouldReturnError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		await customer.CreateCheckoutSession(productService);

		// Act
		var result = await customer.CreateCheckoutSession(productService);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Should().Be(CustomerErrors.ActiveCheckoutSessionExists);
	}

	[Fact]
	public async Task CancelCheckoutSession_WithActiveSession_ShouldCancelSession()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		await customer.CreateCheckoutSession(productService);

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
	public async Task PrepareForCheckout_WithValidAddressAndPaymentType_ShouldPrepareCheckout()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		customer.AddAddress(Constants.Address.DefaultAddress);
		var addressId = customer.Addresses.First().Id;

		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		await customer.CreateCheckoutSession(productService);

		// Act
		var result = customer.PrepareForCheckout(addressId);

		// Assert
		result.IsError.Should().BeFalse();
		customer.ActiveCheckoutSession!.AddressId.Should().Be(addressId);
	}

	[Fact]
	public void PrepareForCheckout_WithNoActiveCheckoutSession_ShouldReturnError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		customer.AddAddress(Constants.Address.DefaultAddress);
		var addressId = customer.Addresses.First().Id;
		// No checkout session created

		// Act
		var result = customer.PrepareForCheckout(addressId);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Should().Be(CustomerErrors.NoActiveCheckoutSession);
	}

	[Fact]
	public void PrepareForCheckout_WithNoCart_ShouldReturnCartNotFoundError()
	{
		// Arrange
		var customer = CustomerFactory.CreateWithAddress();
		var addressId = customer.Addresses.First().Id;

		// Act
		var result = customer.PrepareForCheckout(addressId);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Should().Be(CartErrors.CartNotFound);
	}

	[Fact]
	public async Task PrepareForCheckout_WithNonExistentAddress_ShouldReturnAddressNotFoundError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var basePrice = Constants.Product.DefaultPrice;

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, basePrice);
		productService.GetProductsDetailsAsync(
			Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(productId)),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		await customer.CreateCheckoutSession(productService);
		var nonExistentAddressId = Guid.NewGuid();

		// Act
		var result = customer.PrepareForCheckout(nonExistentAddressId);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Should().Be(CustomerErrors.AddressNotFound);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithMissingProducts_ShouldReturnNoProductsFoundError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);

		var productService = Substitute.For<IProductService>();
		productService.GetProductsDetailsAsync(
			Arg.Any<IReadOnlyList<Guid>>(),
			Arg.Any<CancellationToken>())
			.Returns((List<ProductResponse>?)null);

		// Act
		var result = await customer.CreateCheckoutSession(productService);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.NoProductsFoundForCartItems.Code);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithEmptyProductsList_ShouldReturnNoProductsFoundError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);

		var productService = Substitute.For<IProductService>();
		productService.GetProductsDetailsAsync(
			Arg.Any<IReadOnlyList<Guid>>(),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse>());

		// Act
		var result = await customer.CreateCheckoutSession(productService);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.NoProductsFoundForCartItems.Code);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithSomeProductsMissing_ShouldReturnNoProductFoundError()
	{
		// Arrange
		var merchantId = Constants.Merchant.Id;
		var customer = CustomerFactory.CreateWithCart(merchantId);
		var productId = Constants.Product.Id;
		var differentProductId = Guid.NewGuid();

		// Add another product to cart
		customer.SetCartItem(merchantId, differentProductId, 1);

		var productService = Substitute.For<IProductService>();
		var productResponse = new ProductResponse(productId, "Test Product", merchantId, Constants.Product.DefaultPrice);
		// Only return one product, missing the second one
		productService.GetProductsDetailsAsync(
			Arg.Any<IReadOnlyList<Guid>>(),
			Arg.Any<CancellationToken>())
			.Returns(new List<ProductResponse> { productResponse });

		// Act
		var result = await customer.CreateCheckoutSession(productService);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.NoProductFoundForCartItem(differentProductId).Code);
	}
}
