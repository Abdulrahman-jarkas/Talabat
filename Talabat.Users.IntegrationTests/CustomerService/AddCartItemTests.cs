using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.IntegrationTests.Infrastructure;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class AddCartItemTests : IClassFixture<UsersApiFactory>
{
    private readonly UsersApiFactory _factory;

    public AddCartItemTests(UsersApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddCartItemAsync_WithValidProductAndNoExistingCart_ShouldCreateCartAndAddItem()
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

        var repository = new UsersRepository(_factory.DbContext);
        var customerService = new Users.CustomerService(_factory.MockMediator, repository);

        // Act
        var result = await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        var customer = await _factory.DbContext.Customers
            .Include(c => c.Cart)
            .ThenInclude(cart => cart!.Items)
            .FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

        customer.Should().NotBeNull();
        customer!.Cart.Should().NotBeNull();
        customer.Cart!.Items.Should().HaveCount(1);
        customer.Cart.Items.First().ProductId.Should().Be(productId);
        customer.Cart.Items.First().Quantity.Should().Be(quantity);
        customer.Cart.MerchantId.Should().Be(merchantId);
    }

    [Fact]
    public async Task AddCartItemAsync_WithExistingCartItem_ShouldUpdateQuantity()
    {
        // Arrange
        var productId = Constants.Product.Id;
        var initialQuantity = Constants.Product.DefaultQuantity;
        var updatedQuantity = Constants.Product.UpdatedQuantity;
        var merchantId = Constants.Merchant.Id;

        var productResponse = new ProductResponse(
            productId,
            Constants.Product.Title,
            merchantId,
            Constants.Product.BasePrice);

        _factory.SetupProductQuery(productId, productResponse);

        var repository = new UsersRepository(_factory.DbContext);
        var customerService = new Users.CustomerService(_factory.MockMediator, repository);

        await customerService.AddCartItemAsync(productId, initialQuantity, CancellationToken.None);

        // Act
        var result = await customerService.AddCartItemAsync(productId, updatedQuantity, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        var customer = await _factory.DbContext.Customers
            .Include(c => c.Cart)
            .ThenInclude(cart => cart!.Items)
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

        var repository = new UsersRepository(_factory.DbContext);
        var customerService = new Users.CustomerService(_factory.MockMediator, repository);

        // Act
        var result = await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Contain("NoProductFoundForCartItem");
    }

    [Fact]
    public async Task AddCartItemAsync_WithDifferentMerchant_ShouldReturnMerchantMismatchError()
    {
        // Arrange
        var firstProductId = Constants.Product.Id;
        var secondProductId = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;
        var firstMerchantId = Constants.Merchant.Id;
        var secondMerchantId = Guid.NewGuid();

        var firstProductResponse = new ProductResponse(
            firstProductId,
            Constants.Product.Title,
            firstMerchantId,
            Constants.Product.BasePrice);

        var secondProductResponse = new ProductResponse(
            secondProductId,
            "Another Product",
            secondMerchantId,
            Constants.Product.BasePrice);

        _factory.SetupProductQuery(firstProductId, firstProductResponse);
        _factory.SetupProductQuery(secondProductId, secondProductResponse);

        var repository = new UsersRepository(_factory.DbContext);
        var customerService = new Users.CustomerService(_factory.MockMediator, repository);

        await customerService.AddCartItemAsync(firstProductId, quantity, CancellationToken.None);

        // Act
        var result = await customerService.AddCartItemAsync(secondProductId, quantity, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Contain("MerchantMismatch");
    }
}
