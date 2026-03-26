using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.OrderProcessing.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.Integration;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.Integration;

public class OnOrderPlacedEventHandlerTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private OnOrderPlacedEventHandler _handler = null!;

	public OnOrderPlacedEventHandlerTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

	public async Task InitializeAsync()
	{
		await _factory.ResetDatabaseAsync();
		_handler = new OnOrderPlacedEventHandler(
			new Data.Repositories.UsersRepository(_factory.DbContext));
	}

	public Task DisposeAsync() => Task.CompletedTask;

	[Fact]
	public async Task Handle_WithValidOrder_ShouldSetOrderIdInCheckoutSession()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var orderId = Guid.NewGuid();
		var paymentId = Constants.Payment.PaymentId;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		var productsResponse = new List<ProductResponse> { productResponse };

		// Create customer with active checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get the checkout session ID
		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var checkoutSessionId = customer!.CheckoutSessions.First().Id;

		var orderPlacedEvent = new OnOrderPlacedEvent(orderId, paymentId, checkoutSessionId, Constants.Customer.Id);

		// Act
		await _handler.Handle(orderPlacedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var updatedCustomer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		updatedCustomer.Should().NotBeNull();
		
		var checkoutSession = updatedCustomer!.CheckoutSessions.FirstOrDefault(cs => cs.Id == checkoutSessionId);
		checkoutSession.Should().NotBeNull();
		checkoutSession!.OrderId.Should().Be(orderId);
	}

	[Fact]
	public async Task Handle_WithNonExistentCheckoutSession_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var orderId = Guid.NewGuid();
		var paymentId = Constants.Payment.PaymentId;
		var nonExistentCheckoutSessionId = Guid.NewGuid();

		var orderPlacedEvent = new OnOrderPlacedEvent(orderId, paymentId, nonExistentCheckoutSessionId, Constants.Customer.Id);

		// Act
		var act = async () => await _handler.Handle(orderPlacedEvent, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*OnOrderPlaced.CustomerNotFound*");
	}

	[Fact]
	public async Task Handle_WithMismatchedCustomerId_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var orderId = Guid.NewGuid();
		var paymentId = Constants.Payment.PaymentId;
		var wrongCustomerId = Guid.NewGuid(); // Different customer ID

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		var productsResponse = new List<ProductResponse> { productResponse };

		// Create customer with active checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get the checkout session ID
		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var checkoutSessionId = customer!.CheckoutSessions.First().Id;

		// Create event with wrong customer ID
		var orderPlacedEvent = new OnOrderPlacedEvent(orderId, paymentId, checkoutSessionId, wrongCustomerId);

		// Act
		var act = async () => await _handler.Handle(orderPlacedEvent, CancellationToken.None);

		// Assert - Should throw exception for security violation
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*OnOrderPlaced.CustomerIdMismatch*");
	}

	[Fact]
	public async Task Handle_WithValidOrder_ShouldPersistChangesToDatabase()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var orderId = Guid.NewGuid();
		var paymentId = Constants.Payment.PaymentId;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		var productsResponse = new List<ProductResponse> { productResponse };

		// Create customer with active checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get the checkout session ID
		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var checkoutSessionId = customer!.CheckoutSessions.First().Id;

		var orderPlacedEvent = new OnOrderPlacedEvent(orderId, paymentId, checkoutSessionId, Constants.Customer.Id);

		// Act
		await _handler.Handle(orderPlacedEvent, CancellationToken.None);

		// Assert - Create a new DbContext to verify persistence
		_factory.DbContext.ChangeTracker.Clear();

		var newCustomer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		newCustomer.Should().NotBeNull();
		
		var persistedCheckoutSession = newCustomer!.CheckoutSessions
			.FirstOrDefault(cs => cs.Id == checkoutSessionId);
		
		persistedCheckoutSession.Should().NotBeNull();
		persistedCheckoutSession!.OrderId.Should().Be(orderId);
	}

	[Fact]
	public async Task Handle_WithCompletedCheckoutSession_ShouldStillSetOrderId()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var orderId = Guid.NewGuid();
		var paymentId = Constants.Payment.PaymentId;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		var productsResponse = new List<ProductResponse> { productResponse };

		// Create customer with checkout session and complete it
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get customer and complete the checkout session
		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
				.ThenInclude(cs => cs.Items)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var checkoutSession = customer!.CheckoutSessions.First();
		checkoutSession.Complete();
		await _factory.DbContext.SaveChangesAsync();

		_factory.DbContext.ChangeTracker.Clear();

		var checkoutSessionId = checkoutSession.Id;
		var orderPlacedEvent = new OnOrderPlacedEvent(orderId, paymentId, checkoutSessionId, Constants.Customer.Id);

		// Act
		await _handler.Handle(orderPlacedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var updatedCustomer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		updatedCustomer.Should().NotBeNull();
		
		var updatedCheckoutSession = updatedCustomer!.CheckoutSessions
			.FirstOrDefault(cs => cs.Id == checkoutSessionId);
		
		updatedCheckoutSession.Should().NotBeNull();
		updatedCheckoutSession!.OrderId.Should().Be(orderId);
		updatedCheckoutSession.Status.Should().Be(CheckoutSessionStatus.Completed);
	}

	[Fact]
	public async Task Handle_WhenCustomerHasMultipleCheckoutSessions_ShouldOnlyUpdateSpecifiedSession()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var orderId = Guid.NewGuid();
		var paymentId = Constants.Payment.PaymentId;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		var productsResponse = new List<ProductResponse> { productResponse };

		// Create customer with first checkout session
		var customerService = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);

		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get first checkout session and complete it
		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
				.ThenInclude(cs => cs.Items)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var firstCheckoutSessionId = customer!.CheckoutSessions.First().Id;
		customer.CheckoutSessions.First().Complete();
		await _factory.DbContext.SaveChangesAsync();

		_factory.DbContext.ChangeTracker.Clear();

		// Create second checkout session
		await TestHelper.AddProductToCartAsync(customerService, productId, quantity);
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get second checkout session
		customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var secondCheckoutSessionId = customer!.CheckoutSessions
			.First(cs => cs.Status == CheckoutSessionStatus.Active).Id;

		var orderPlacedEvent = new OnOrderPlacedEvent(orderId, paymentId, firstCheckoutSessionId, Constants.Customer.Id);

		// Act
		await _handler.Handle(orderPlacedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var updatedCustomer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		updatedCustomer.Should().NotBeNull();
		updatedCustomer!.CheckoutSessions.Should().HaveCount(2);

		var firstSession = updatedCustomer.CheckoutSessions
			.First(cs => cs.Id == firstCheckoutSessionId);
		var secondSession = updatedCustomer.CheckoutSessions
			.First(cs => cs.Id == secondCheckoutSessionId);

		firstSession.OrderId.Should().Be(orderId);
		secondSession.OrderId.Should().BeNull("only the specified session should be updated");
	}
}
