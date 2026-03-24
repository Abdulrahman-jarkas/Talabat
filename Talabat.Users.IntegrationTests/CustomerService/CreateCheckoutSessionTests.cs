using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class CreateCheckoutSessionTests : IClassFixture<UsersApiFactory>
{
	private readonly UsersApiFactory _factory;

	public CreateCheckoutSessionTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

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

		_factory.SetupProductQuery(productId, productResponse);
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponse);

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

		// Act
		var result = await customerService.CreateCheckoutSession(CancellationToken.None);

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
		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		// Act
		var result = await customerService.CreateCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("CartItems.ProductsNotFound");
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

		_factory.SetupProductQuery(productId, productResponse);
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponse);

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		// Act
		var result = await customerService.CreateCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("ActiveCheckoutSessionExists");
	}

	[Fact]
	public async Task CreateCheckoutSession_WithMissingProducts_ShouldReturnNoProductsFoundError()
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
		_factory.SetupProductsQuery(new List<Guid> { productId }, null);

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

		// Act
		var result = await customerService.CreateCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("CartItems.ProductsNotFound");
	}
}
