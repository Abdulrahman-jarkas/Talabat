using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Application.Customer.Commands.AddCartItem;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class AddCartItemTests : IClassFixture<UsersApiFactory>, IAsyncLifetime
{
	private readonly UsersApiFactory _factory;
	private AddCartItemCommandHandler _handler = null!;

	public AddCartItemTests(UsersApiFactory factory)
	{
		_factory = factory;
	}

	public async Task InitializeAsync()
	{
		await _factory.ResetDatabaseAsync();
		_handler = TestHelper.CreateAddCartItemHandler(_factory);
	}

	public Task DisposeAsync() => Task.CompletedTask;

	[Fact]
	public async Task AddCartItemAsync_WithValidProductAndNoExistingCart_ShouldCreateCartAndAddItem()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;
		var shopId = Constants.Shop.Id;

		var productResponse = new ProductResponse(
			productId,
			Constants.Product.Title,
			shopId,
			Constants.Product.BasePrice,
			100);

		_factory.SetupProductQuery(productId, productResponse);

		// Act
		var result = await _handler.Handle(
			new AddCartItemCommand(Constants.Customer.Id, productId, quantity),
			CancellationToken.None);
		_factory.DbContext.ChangeTracker.Clear();


		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer.Should().NotBeNull();
		customer!.Cart.Should().NotBeNull();
		customer.Cart!.Items.Should().HaveCount(1);
		customer.Cart.Items.First().ProductId.Should().Be(productId);
		customer.Cart.Items.First().Quantity.Should().Be(quantity);
		customer.Cart.ShopId.Should().Be(shopId);
	}

	[Fact]
	public async Task AddCartItemAsync_WithExistingCartItem_ShouldUpdateQuantity()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var initialQuantity = Constants.Product.DefaultQuantity;
		var updatedQuantity = Constants.Product.UpdatedQuantity;
		var shopId = Constants.Shop.Id;

		// Setup product mock and add initial cart item
		TestHelper.SetupProductQuery(_factory, productId, shopId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_handler, productId, initialQuantity);

		// Act
		var result = await _handler.Handle(
			new AddCartItemCommand(Constants.Customer.Id, productId, updatedQuantity),
			CancellationToken.None);
		_factory.DbContext.ChangeTracker.Clear();


		// Assert
		result.IsError.Should().BeFalse();

		var customer = await _factory.DbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

		customer!.Cart!.Items.Should().HaveCount(1);
		customer.Cart.Items.First().Quantity.Should().Be(updatedQuantity);
	}

	[Fact]
	public async Task AddCartItemAsync_WithNonExistentProduct_ShouldReturnError()
	{
		// Arrange
		var productId = Constants.Product.Id;
		var quantity = Constants.Product.DefaultQuantity;

		_factory.SetupProductQuery(productId, null);

		// Act
		var result = await _handler.Handle(
			new AddCartItemCommand(Constants.Customer.Id, productId, quantity),
			CancellationToken.None);

		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CartErrors.NoProductFoundForCartItem(productId).Code);
	}

	[Fact]
	public async Task AddCartItemAsync_WithDifferentShop_ShouldReturnShopMismatchError()
	{
		// Arrange
		var firstProductId = Constants.Product.Id;
		var secondProductId = Constants.Product.AlternativeId;
		var quantity = Constants.Product.DefaultQuantity;
		var firstshopId = Constants.Shop.Id;
		var secondshopId = Guid.NewGuid();

		// Setup first product and add to cart
		TestHelper.SetupProductQuery(_factory, firstProductId, firstshopId, Constants.Product.BasePrice);
		await TestHelper.AddProductToCartAsync(_handler, firstProductId, quantity);

		// Setup second product with different Shop
		TestHelper.SetupProductQuery(_factory, secondProductId, secondshopId, Constants.Product.BasePrice);

		// Act
		var result = await _handler.Handle(
			new AddCartItemCommand(Constants.Customer.Id, secondProductId, quantity),
			CancellationToken.None);


		// Assert
		result.IsError.Should().BeTrue();
		result.FirstError.Code.Should().Be(CustomerErrors.ShopMismatch.Code);
	}
}


