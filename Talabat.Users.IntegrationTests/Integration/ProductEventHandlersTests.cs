using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.ProductsManagement.Contracts;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.Integration;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;
using ProductResponse = Talabat.Products.Contracts.ProductResponse;

namespace Talabat.Users.IntegrationTests.Integration;

public class ProductEventHandlersTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private ProductDeletedEventHandler _deletedHandler = null!;
	private ProductUpdatedEventHandler _updatedHandler = null!;

	public ProductEventHandlersTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

	public async Task InitializeAsync()
	{
		await _factory.ResetDatabaseAsync();
		var repository = new Data.Repositories.UsersRepository(_factory.DbContext);
		_deletedHandler = new ProductDeletedEventHandler(repository);
		_updatedHandler = new ProductUpdatedEventHandler(repository);
	}

	public Task DisposeAsync() => Task.CompletedTask;

	#region ProductDeletedEventHandler Tests

	[Fact]
	public async Task ProductDeleted_WithCustomerHavingProductInCartAndActiveCheckout_ShouldResetCheckoutSession()
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

		// Create customer with product in cart and active checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		var productDeletedEvent = new ProductDeletedEvent(productId);

		// Act
		await _deletedHandler.Handle(productDeletedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.ActiveCheckoutSession.Should().BeNull("checkout session should be reset when product is deleted");
		customer.CheckoutSessions.Should().BeEmpty("reset removes the checkout session");
	}

	[Fact]
	public async Task ProductDeleted_WithCustomerHavingProductButNoActiveCheckout_ShouldNotThrowException()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		// Create customer with product in cart but no checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		_factory.DbContext.ChangeTracker.Clear();

		var productDeletedEvent = new ProductDeletedEvent(productId);

		// Act
		var act = async () => await _deletedHandler.Handle(productDeletedEvent, CancellationToken.None);

		// Assert
		await act.Should().NotThrowAsync();

		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.CheckoutSessions.Should().BeEmpty("no checkout session to reset");
	}

	[Fact]
	public async Task ProductDeleted_WithNoCustomersHavingProduct_ShouldNotThrowException()
	{
		// Arrange
		var nonExistentProductId = Guid.NewGuid();
		var productDeletedEvent = new ProductDeletedEvent(nonExistentProductId);

		// Act
		var act = async () => await _deletedHandler.Handle(productDeletedEvent, CancellationToken.None);

		// Assert
		await act.Should().NotThrowAsync();
	}

	#endregion

	#region ProductUpdatedEventHandler Tests

	[Fact]
	public async Task ProductUpdated_WithCustomerHavingProductInCartAndActiveCheckout_ShouldResetCheckoutSession()
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

		// Create customer with product in cart and active checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		var productUpdatedEvent = new ProductUpdatedEvent(productId);

		// Act
		await _updatedHandler.Handle(productUpdatedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.ActiveCheckoutSession.Should().BeNull("checkout session should be reset when product is updated");
		customer.CheckoutSessions.Should().BeEmpty("reset removes the checkout session");
	}

	[Fact]
	public async Task ProductUpdated_WithCustomerHavingProductButNoActiveCheckout_ShouldNotThrowException()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;

		// Create customer with product in cart but no checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		_factory.DbContext.ChangeTracker.Clear();

		var productUpdatedEvent = new ProductUpdatedEvent(productId);

		// Act
		var act = async () => await _updatedHandler.Handle(productUpdatedEvent, CancellationToken.None);

		// Assert
		await act.Should().NotThrowAsync();

		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.CheckoutSessions.Should().BeEmpty("no checkout session to reset");
	}

	[Fact]
	public async Task ProductUpdated_WithNoCustomersHavingProduct_ShouldNotThrowException()
	{
		// Arrange
		var nonExistentProductId = Guid.NewGuid();
		var productUpdatedEvent = new ProductUpdatedEvent(nonExistentProductId);

		// Act
		var act = async () => await _updatedHandler.Handle(productUpdatedEvent, CancellationToken.None);

		// Assert
		await act.Should().NotThrowAsync();
	}

	#endregion
}
