using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;

namespace Talabat.Users.IntegrationTests.TestUtils;

internal static class TestHelper
{
	/// <summary>
	/// Creates a customer service for testing.
	/// Assumes customer is already seeded by ResetDatabaseAsync.
	/// </summary>
	internal static Users.CustomerService CreateCustomerServiceAsync(
		UsersApiFactory factory)
	{
		var repository = new UsersRepository(factory.DbContext);
		var service = new Users.CustomerService(
			Constants.Customer.Id,
			factory.MockMediator,
			repository);
		return service;
	}

	/// <summary>
	/// Adds a product to the customer's cart.
	/// Note: Test must call SetupProductQuery before this to mock the product service.
	/// </summary>
	internal static async Task AddProductToCartAsync(
		Users.CustomerService customerService,
		Guid? productId = null,
		int? quantity = null)
	{
		var prodId = productId ?? Constants.Product.Id;
		var qty = quantity ?? Constants.Product.DefaultQuantity;

		await customerService.AddCartItemAsync(prodId, qty, CancellationToken.None);
	}

	/// <summary>
	/// Helper to setup product query mock for a single product.
	/// </summary>
	internal static void SetupProductQuery(
		UsersApiFactory factory,
		Guid productId,
		Guid merchantId,
		decimal price)
	{
		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			merchantId,
			price,
			100);

		factory.SetupProductQuery(productId, productResponse);
	}

	/// <summary>
	/// Helper to setup products query mock for multiple products.
	/// </summary>
	internal static void SetupProductsQuery(
		UsersApiFactory factory,
		List<Guid> productIds,
		List<ProductResponse> products)
	{
		factory.SetupProductsQuery(productIds, products);
	}

	/// <summary>
	/// Adds an address to the test customer.
	/// </summary>
	internal static async Task<Guid> AddAddressToCustomerAsync(
		UsersApiFactory factory,
		string? address = null,
		Guid? customerId = null)
	{
		var custId = customerId ?? Constants.Customer.Id;

		var customer = await factory.DbContext.Customers
			.Include(c => c.Addresses)
			.FirstOrDefaultAsync(c => c.Id == custId);

		var addressValue = address ?? Constants.Address.DefaultAddress;
		customer!.AddAddress(addressValue);

		await factory.DbContext.SaveChangesAsync();

		return customer.Addresses.Last().Id;
	}
}
