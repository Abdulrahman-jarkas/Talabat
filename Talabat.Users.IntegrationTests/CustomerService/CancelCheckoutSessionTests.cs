using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class CancelCheckoutSessionTests : IClassFixture<UsersApiFactory>
{
	private readonly UsersApiFactory _factory;

	public CancelCheckoutSessionTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

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

		_factory.SetupProductQuery(productId, productResponse);
		_factory.SetupProductsQuery(new List<Guid> { productId }, productsResponse);

		var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

		await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);
		await customerService.CreateCheckoutSession(CancellationToken.None);

		// Act
		var result = await customerService.CancelCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.ActiveCheckoutSession.Should().BeNull();
	}

	[Fact]
	public async Task CancelCheckoutSession_WithNoActiveSession_ShouldReturnNoActiveCheckoutSessionError()
	{
		// Arrange
		var (customerService, _) = await TestHelper.CreateCustomerServiceAsync(_factory);

		// Act
		var result = await customerService.CancelCheckoutSession(CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Contain("NoActiveCheckoutSession");
	}
}
