using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class AddCartItemTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private Users.CustomerService _customerService = null!;

	public AddCartItemTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

	public async Task InitializeAsync()
	{
		await _factory.ResetDatabaseAsync();
		_customerService = TestHelper.CreateCustomerServiceAsync(_factory);
	}

	public Task DisposeAsync() => Task.CompletedTask;

	[Fact]
	public async Task AddCartItemAsync_WithValidProductAndNoExistingCart_ShouldCreateCartAndAddItem()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		_factory.SetupProductQuery(productId, productResponse);

		// Act
		var result = await _customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.Cart.Should().NotBeNull();
		customer.Cart!.Items.Should().HaveCount(1);
		customer.Cart.Items.First().ProductId.Should().Be(productId);
		customer.Cart.Items.First().Quantity.Should().Be(quantity);
		customer.Cart.MerchantId.Should().Be(merchantId);
	}

	[Fact]
	public async Task AddCartItemAsync_WithExistingCartItem_ShouldUpdateQuantity()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var initialQuantity = Constants.Product.DefaultQuantity;
		var updatedQuantity = Constants.Product.UpdatedQuantity;
		var merchantId = Constants.Merchant.Id;

		// Setup product mock and add initial cart item
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, initialQuantity);

		// Setup mock again for update operation
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);

		// Act
		var result = await _customerService.AddCartItemAsync(productId, updatedQuantity, CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.Cart!.Items.Should().HaveCount(1);
		customer.Cart.Items.First().Quantity.Should().Be(updatedQuantity);
	}

	[Fact]
	public async Task AddCartItemAsync_WithNonExistentProduct_ShouldReturnError()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;

		_factory.SetupProductQuery(productId, null);

		// Act
		var result = await _customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.NoProductFoundForCartItem(productId).Code);
	}

	[Fact]
	public async Task AddCartItemAsync_WithDifferentMerchant_ShouldReturnMerchantMismatchError()
	{
		// Arrange
		var firstProductId = Constants.Product.Id;
		var secondProductId = Constants.Product.AlternativeId;
		var quantity = Constants.Product.DefaultQuantity;
		var firstMerchantId = Constants.Merchant.Id;
		var secondMerchantId = Guid.NewGuid();

		// Setup first product and add to cart
		TestHelper.SetupProductQuery(_factory, firstProductId, firstMerchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, firstProductId, quantity);

		// Setup second product with different merchant
		TestHelper.SetupProductQuery(_factory, secondProductId, secondMerchantId, Constants.Product.BasePrice);

		// Act
		var result = await _customerService.AddCartItemAsync(secondProductId, quantity, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CustomerErrors.MerchantMismatch.Code);
	}
}
