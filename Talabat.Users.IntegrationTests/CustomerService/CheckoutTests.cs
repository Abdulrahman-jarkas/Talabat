using ErrorOr;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class CheckoutTests : IClassFixture<UsersApiFactory>
{
	private readonly UsersApiFactory _factory;

	public CheckoutTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

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

		_factory.SetupProductQuery(productId, productResponse);
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponse);

		var paymentResponse = new CreatePaymentSessionResponseDto
		{
			PaymentId = Constants.Payment.PaymentId,
			PaymentUrl = Constants.Payment.PaymentUrl
		};

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		var customer = await _factory.DbContext.Customers
			.Include(c => c.Addresses)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.AddAddress(Constants.Address.DefaultAddress);
		await _factory.DbContext.SaveChangesAsync();

		var addressId = customer.Addresses.First().Id;

		var checkoutSession = await _factory.DbContext.Customers
			.Where(c => c.Id == Constants.Customer.Id)
			.Select(c => c.ActiveCheckoutSession)
			.FirstOrDefaultAsync();

		_factory.SetupCreatePaymentSession(
			Constants.Customer.Id,
			checkoutSession!.Id,
			expectedTotalPrice,
			paymentResponse);

		// Act
		var result = await customerService.Checkout(addressId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();
		result.Value.PaymentId.Should().Be(Constants.Payment.PaymentId);
		result.Value.PaymentUrl.Should().Be(Constants.Payment.PaymentUrl);
	}

	[Fact]
	public async Task Checkout_WithoutActiveCheckoutSession_ShouldReturnCheckoutSessionNotFoundError()
	{
		// Arrange
		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		var customer = await _factory.DbContext.Customers
			.Include(c => c.Addresses)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.AddAddress(Constants.Address.DefaultAddress);
		await _factory.DbContext.SaveChangesAsync();

		var addressId = customer.Addresses.First().Id;

		// Act
		var result = await customerService.Checkout(addressId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("Cart.NotFound");
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

		_factory.SetupProductQuery(productId, productResponse);
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponse);

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		// Act
		var result = await customerService.Checkout(invalidAddressId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("AddressNotFound");
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

		_factory.SetupProductQuery(productId, productResponseOriginal);
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponseOriginal);

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		var customer = await _factory.DbContext.Customers
			.Include(c => c.Addresses)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.AddAddress(Constants.Address.DefaultAddress);
		await _factory.DbContext.SaveChangesAsync();

		var addressId = customer.Addresses.First().Id;

		// Simulate price change before checkout
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponseChanged);

		// Act
		var result = await customerService.Checkout(addressId, CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("PriceMismatch");
	}
}
