using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class CancelCheckoutSessionTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private Users.CustomerService _customerService = null!;

	public CancelCheckoutSessionTests(UsersApiFactory factory)
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
	public async Task CancelCheckoutSession_WithActiveSession_ShouldCancelSuccessfully()
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
		var result = await _customerService.CancelCheckoutSession(CancellationToken.None);

		// Assert
		//result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.Include(c => c.ActiveCheckoutSession)
				.ThenInclude(cs => cs!.Items)
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.ActiveCheckoutSession.Should().BeNull();
	}

	[Fact]
	public async Task CancelCheckoutSession_WithNoActiveSession_ShouldReturnNoActiveCheckoutSessionError()
	{
		// Arrange
		// Use class-level _customerService

		// Act
		var result = await _customerService.CancelCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CustomerErrors.NoActiveCheckoutSession.Code);
	}
}
