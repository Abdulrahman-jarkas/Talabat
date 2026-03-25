using Docker.DotNet.Models;
using ErrorOr;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class CheckoutTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private Users.CustomerService _customerService = null!;

	public CheckoutTests(UsersApiFactory factory)
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
	public async Task Checkout_WithValidData_ShouldReturnPaymentIdAndUrl()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var expectedTotalPrice = Constants.Product.BasePrice * quantity;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			Constants.Product.BasePrice);

		var productsResponse = new List<ProductResponse> { productResponse };

		var paymentResponse = new CreatePaymentSessionResponseDto
		{
			PaymentId = Constants.Payment.PaymentId,
			PaymentUrl = Constants.Payment.PaymentUrl
		};

		// Setup product mock and add to cart
		TestHelper.SetupProductQuery(_factory, productId, merchantId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, quantity);

		// Setup products query for checkout session
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponse);
		await _customerService.CreateCheckoutSession(CancellationToken.None);

		var addressId = await TestHelper.AddAddressToCustomerAsync(_factory);

		_factory.DbContext.ChangeTracker.Clear();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.CheckoutSessions)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		var checkoutSession = customer!.ActiveCheckoutSession;

		_factory.SetupCreatePaymentSession(
			Constants.Customer.Id,
			checkoutSession!.Id,
			expectedTotalPrice,
			paymentResponse);

		// Act
		_factory.DbContext.ChangeTracker.Clear();

		var result = await _customerService.Checkout(addressId, PaymentType.Online, CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();
		result.Value.PaymentId.Should().Be(Constants.Payment.PaymentId);
		result.Value.PaymentUrl.Should().Be(Constants.Payment.PaymentUrl);
	}

	[Fact]
	public async Task Checkout_WithoutActiveCheckoutSession_ShouldReturnCheckoutSessionNotFoundError()
	{
		// Arrange
		var addressId = await TestHelper.AddAddressToCustomerAsync(_factory);

		// Act
		_factory.DbContext.ChangeTracker.Clear();
		var result = await _customerService.Checkout(addressId, PaymentType.Online, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.CartNotFound.Code);
	}

	[Fact]
	public async Task Checkout_WithInvalidAddress_ShouldReturnAddressNotFoundError()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var invalidAddressId = Guid.NewGuid();

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
		var result = await _customerService.Checkout(invalidAddressId, PaymentType.Online, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CustomerErrors.AddressNotFound.Code);
	}

	[Fact]
	public async Task Checkout_WithPriceMismatch_ShouldReturnPriceMismatchError()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var merchantId = Constants.Merchant.Id;
		var originalPrice = Constants.Product.BasePrice;
		var changedPrice = Constants.Product.BasePrice + 50;

		var productResponseOriginal = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			originalPrice);

		var productResponseChanged = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			changedPrice);

		var productsResponseOriginal = new List<ProductResponse> { productResponseOriginal };
		var productsResponseChanged = new List<ProductResponse> { productResponseChanged };

		// Setup product mock with original price and add to cart
		TestHelper.SetupProductQuery(_factory, productId, merchantId, originalPrice);
		await TestHelper.AddProductToCartAsync(_customerService, productId, quantity);

		// Setup products query for checkout session with original price
		TestHelper.SetupProductsQuery(_factory, new List<Guid> { productId }, productsResponseOriginal);
		await _customerService.CreateCheckoutSession(CancellationToken.None);

		var addressId = await TestHelper.AddAddressToCustomerAsync(_factory);


		// Simulate price change before checkout
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponseChanged);

		// Act
		_factory.DbContext.ChangeTracker.Clear();
		var result = await _customerService.Checkout(addressId, PaymentType.Online, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CheckoutSessionErrors.PriceMismatch.Code);
	}
}
