using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.Integration;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.Integration;

public class PaymentSuccessedEventHandlerTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private PaymentSuccessedEventHandler _handler = null!;

	public PaymentSuccessedEventHandlerTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

	public async Task InitializeAsync()
	{
		await _factory.ResetDatabaseAsync();
		_handler = new PaymentSuccessedEventHandler(
			new Data.Repositories.UsersRepository(_factory.DbContext));
	}

	public Task DisposeAsync() => Task.CompletedTask;

	[Fact]
	public async Task Handle_WithValidPayment_ShouldSetPaymentIdAndCompleteCheckoutSession()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
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

		var paymentSuccessedEvent = new PaymentSuccessedEvent(paymentId, Constants.Customer.Id);

		// Act
		await _handler.Handle(paymentSuccessedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		
		var checkoutSession = customer!.CheckoutSessions.FirstOrDefault();
		checkoutSession.Should().NotBeNull();
		checkoutSession!.PaymentId.Should().Be(paymentId);
		checkoutSession.Status.Should().Be(CheckoutSessionStatus.Completed);
	}

	[Fact]
	public async Task Handle_WithNonExistentCustomer_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var paymentId = Constants.Payment.PaymentId;
		var nonExistentCustomerId = Guid.NewGuid();
		var paymentSuccessedEvent = new PaymentSuccessedEvent(paymentId, nonExistentCustomerId);

		// Act
		var act = async () => await _handler.Handle(paymentSuccessedEvent, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*PaymentSuccessed.CustomerNotFound*");
	}

	[Fact]
	public async Task Handle_WithNoActiveCheckoutSession_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var paymentId = Constants.Payment.PaymentId;

		// Customer is already created by InitializeAsync (ResetDatabaseAsync)
		// but has no checkout session
		_factory.DbContext.ChangeTracker.Clear();

		var paymentSuccessedEvent = new PaymentSuccessedEvent(paymentId, Constants.Customer.Id);

		// Act
		var act = async () => await _handler.Handle(paymentSuccessedEvent, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*PaymentSuccessed.FailedToSetPaymentId*");
	}

	[Fact]
	public async Task Handle_WithValidPayment_ShouldNotChangeActiveCheckoutSessionProperty()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
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

		var paymentSuccessedEvent = new PaymentSuccessedEvent(paymentId, Constants.Customer.Id);

		// Act
		await _handler.Handle(paymentSuccessedEvent, CancellationToken.None);

		// Assert - Verify that ActiveCheckoutSession returns null because the session is now completed
		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions.Where(cs => cs.Status == CheckoutSessionStatus.Active))
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.ActiveCheckoutSession.Should().BeNull("the checkout session should be completed");
	}

	[Fact]
	public async Task Handle_WithValidPayment_ShouldPersistChangesToDatabase()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
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

		var checkoutSessionIdBeforeEvent = (await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id))!
			.CheckoutSessions.First().Id;

		var paymentSuccessedEvent = new PaymentSuccessedEvent(paymentId, Constants.Customer.Id);

		// Act
		await _handler.Handle(paymentSuccessedEvent, CancellationToken.None);

		// Assert - Create a new DbContext to ensure we're reading from database
		var options = new DbContextOptionsBuilder<Data.UsersDbContext>()
			.UseNpgsql(_factory.DbContext.Database.GetConnectionString())
			.Options;

		await using var newDbContext = new Data.UsersDbContext(options);

		var checkoutSession = await newDbContext.Set<CheckoutSession>()
			.FirstOrDefaultAsync(cs => cs.Id == checkoutSessionIdBeforeEvent);

		checkoutSession.Should().NotBeNull();
		checkoutSession!.PaymentId.Should().Be(paymentId);
		checkoutSession.Status.Should().Be(CheckoutSessionStatus.Completed);
	}
}
