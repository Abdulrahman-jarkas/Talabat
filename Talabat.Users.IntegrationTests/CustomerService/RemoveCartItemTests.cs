using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class RemoveCartItemTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private Users.CustomerService _customerService = null!;

	public RemoveCartItemTests(UsersApiFactory factory)
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
	public async Task RemoveCartItemAsync_WithExistingItem_ShouldRemoveItemFromCart()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		// Set up product mock and add to cart
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, quantity);

		// Act
		var result = await _customerService.RemoveCartItemAsync(productId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();

		// Detach to force reload from database
		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.Cart.Should().NotBeNull();
		customer.Cart!.Items.Should().BeEmpty();
	}

	[Fact]
	public async Task RemoveCartItemAsync_WithNonExistentItem_ShouldReturnCartItemNotFoundError()
	{
		// Arrange
		var existingProductId = Constants.Product.Id;
		var nonExistentProductId = Constants.Product.AlternativeId;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		// First, create a cart with one item
		TestHelper.SetupProductQuery(_factory, existingProductId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, existingProductId, quantity);

		// Act - Try to remove a different product that doesn't exist in the cart
		var result = await _customerService.RemoveCartItemAsync(nonExistentProductId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.CartItemNotFound.Code);
	}

	[Fact]
	public async Task RemoveCartItemAsync_WithMultipleItems_ShouldOnlyRemoveSpecifiedItem()
	{
		// Arrange
		var firstProductId = Constants.Product.Id;
		var secondProductId = Constants.Product.AlternativeId;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		// Set up both products and add to cart
		TestHelper.SetupProductQuery(_factory, firstProductId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, firstProductId, quantity);

		TestHelper.SetupProductQuery(_factory, secondProductId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, secondProductId, quantity);

		// Act
		var result = await _customerService.RemoveCartItemAsync(firstProductId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.Cart!.Items.Should().HaveCount(1);
		customer.Cart.Items.First().ProductId.Should().Be(secondProductId);
	}
}
