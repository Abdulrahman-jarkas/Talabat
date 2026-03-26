using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.Integration;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.Integration;

public class PaymentFailedEventHandlerTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private PaymentFailedEventHandler _handler = null!;

	public PaymentFailedEventHandlerTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

	public async Task InitializeAsync()
	{
		await _factory.ResetDatabaseAsync();
		_handler = new PaymentFailedEventHandler(
			new Data.Repositories.UsersRepository(_factory.DbContext));
	}

	public Task DisposeAsync() => Task.CompletedTask;

	[Fact]
	public async Task Handle_WithValidPaymentFailure_ShouldCancelActiveCheckoutSession()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var paymentId = Constants.Payment.PaymentId;
		var failureReason = "Insufficient funds";

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

		// Verify checkout session exists before handling event
		var customerBefore = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customerBefore!.CheckoutSessions.Should().HaveCount(1);
		customerBefore.CheckoutSessions.First().Status.Should().Be(CheckoutSessionStatus.Active);

		var paymentFailedEvent = new PaymentFailedEvent(paymentId, Constants.Customer.Id, failureReason);

		// Act
		await _handler.Handle(paymentFailedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var customerAfter = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customerAfter.Should().NotBeNull();
		customerAfter!.CheckoutSessions.Should().BeEmpty("checkout session should be cancelled after payment failure");
	}

	[Fact]
	public async Task Handle_WithNonExistentCustomer_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var paymentId = Constants.Payment.PaymentId;
		var nonExistentCustomerId = Guid.NewGuid();
		var failureReason = "Card declined";
		var paymentFailedEvent = new PaymentFailedEvent(paymentId, nonExistentCustomerId, failureReason);

		// Act
		var act = async () => await _handler.Handle(paymentFailedEvent, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*PaymentFailed.CustomerNotFound*");
	}

	[Fact]
	public async Task Handle_WithNoActiveCheckoutSession_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var paymentId = Constants.Payment.PaymentId;
		var failureReason = "Payment timeout";

		// Customer is already created by InitializeAsync (ResetDatabaseAsync)
		// but has no checkout session
		_factory.DbContext.ChangeTracker.Clear();

		var paymentFailedEvent = new PaymentFailedEvent(paymentId, Constants.Customer.Id, failureReason);

		// Act
		var act = async () => await _handler.Handle(paymentFailedEvent, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("*PaymentFailed.FailedToCancelCheckout*");
	}

	[Fact]
	public async Task Handle_WithValidPaymentFailure_ShouldPersistChangesToDatabase()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var paymentId = Constants.Payment.PaymentId;
		var failureReason = "Payment gateway error";

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

		var paymentFailedEvent = new PaymentFailedEvent(paymentId, Constants.Customer.Id, failureReason);

		// Act
		await _handler.Handle(paymentFailedEvent, CancellationToken.None);

		// Assert - Verify checkout session is removed from customer's collection
		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.CheckoutSessions.Should().BeEmpty("checkout session should be removed after payment failure");
	}

	[Fact]
	public async Task Handle_WithValidPaymentFailure_ShouldAllowCreatingNewCheckoutSessionAfterCancellation()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var paymentId = Constants.Payment.PaymentId;
		var failureReason = "Fraud detection";

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

		var paymentFailedEvent = new PaymentFailedEvent(paymentId, Constants.Customer.Id, failureReason);

		// Act - Cancel checkout due to payment failure
		await _handler.Handle(paymentFailedEvent, CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Act - Try to create new checkout session
		var customerServiceAfterCancellation = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		var createResult = await customerServiceAfterCancellation.CreateCheckoutSession(CancellationToken.None);

		// Assert
		createResult.IsError.Should().BeFalse("should be able to create new checkout session after cancellation");

		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.CheckoutSessions.Should().HaveCount(1, "new checkout session should be created after cancellation");
		customer.ActiveCheckoutSession.Should().NotBeNull("new checkout session should be active");
	}

	[Fact]
	public async Task Handle_WithMultipleCheckoutSessions_ShouldOnlyCancelActiveOne()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var paymentId = Constants.Payment.PaymentId;
		var failureReason = "Network timeout";

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

		// Complete first checkout session manually to create a non-active session
		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var firstCheckoutSession = customer!.CheckoutSessions.First();
		firstCheckoutSession.SetPaymentId(Guid.NewGuid());
		customer.CompleteCheckoutSession();
		await _factory.DbContext.SaveChangesAsync();

		_factory.DbContext.ChangeTracker.Clear();

		// Create second active checkout session
		var customerService2 = TestHelper.CreateCustomerServiceAsync(_factory);
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await customerService2.CreateCheckoutSession(CancellationToken.None);

		_factory.DbContext.ChangeTracker.Clear();

		// Get the active checkout session ID before cancellation
		var customerBefore = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var activeCheckoutSessionId = customerBefore!.CheckoutSessions
			.First(cs => cs.Status == CheckoutSessionStatus.Active).Id;

		customerBefore.CheckoutSessions.Should().HaveCount(2, "should have 2 checkout sessions (1 completed, 1 active)");

		var paymentFailedEvent = new PaymentFailedEvent(paymentId, Constants.Customer.Id, failureReason);

		// Act
		await _handler.Handle(paymentFailedEvent, CancellationToken.None);

		// Assert
		_factory.DbContext.ChangeTracker.Clear();

		var customerAfter = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customerAfter.Should().NotBeNull();
		customerAfter!.CheckoutSessions.Should().HaveCount(1, "only active checkout session should be cancelled");
		customerAfter.CheckoutSessions.First().Status.Should().Be(CheckoutSessionStatus.Completed, "completed session should remain");
		customerAfter.CheckoutSessions.Should().NotContain(cs => cs.Id == activeCheckoutSessionId, "active session should be removed");
	}
}
