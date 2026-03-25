using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class CreateCheckoutSessionTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private Users.CustomerService _customerService = null!;

	public CreateCheckoutSessionTests(UsersApiFactory factory)
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
	public async Task CreateCheckoutSession_WithValidCart_ShouldCreateCheckoutSession()
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

		var productsResponse = new List<ProductResponse> { productResponse };

		// Setup product mock and add to cart
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, quantity);

		// Setup products query for checkout session validation
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);

		// Act
		_factory.DbContext.ChangeTracker.Clear();
		var result = await _customerService.CreateCheckoutSession(CancellationToken.None);
		_factory.DbContext.ChangeTracker.Clear();

		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.ActiveCheckoutSession)
			.ThenInclude(s => s.Items)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.ActiveCheckoutSession.Should().NotBeNull();
		customer.ActiveCheckoutSession!.Items.Should().HaveCount(1);
		customer.ActiveCheckoutSession.Items.First().ProductId.Should().Be(productId);
		customer.ActiveCheckoutSession.Items.First().Quantity.Should().Be(quantity);
		customer.ActiveCheckoutSession.Items.First().BasePrice.Should().Be(Constants.Product.BasePrice);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithEmptyCart_ShouldReturnCartNotFoundError()
	{
		// Arrange
		// Use class-level _customerService

		// Act
		var result = await _customerService.CreateCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.CartNotFound.Code);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithExistingActiveSession_ShouldReturnActiveCheckoutSessionExistsError()
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

		var productsResponse = new List<ProductResponse> { productResponse };

		// Setup product mock and add to cart
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, quantity);

		// Setup products query for checkout session
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await _customerService.CreateCheckoutSession(CancellationToken.None);

		// Act
		_factory.DbContext.ChangeTracker.Clear();
		var result = await _customerService.CreateCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CustomerErrors.ActiveCheckoutSessionExists.Code);
	}

	[Fact]
	public async Task CreateCheckoutSession_WithMissingProducts_ShouldReturnNoProductsFoundError()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		// Setup product mock and add to cart
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, quantity);

		// Setup products query to return null (simulating missing products)
		_factory.SetupProductsQuery(new List<Guid> { productId }, null);

		// Act
		_factory.DbContext.ChangeTracker.Clear();
		var result = await _customerService.CreateCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.NoProductsFoundForCartItems.Code);
	}
}
