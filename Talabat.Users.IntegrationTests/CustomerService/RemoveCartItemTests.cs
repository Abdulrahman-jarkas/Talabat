using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;
using Talabat.Users.IntegrationTests.TestUtils;

namespace Talabat.Users.IntegrationTests.CustomerService;

public class RemoveCartItemTests : IClassFixture<UsersApiFactory>
{
    private readonly UsersApiFactory _factory;

    public RemoveCartItemTests(UsersApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RemoveCartItemAsync_WithExistingItem_ShouldRemoveItemFromCart()
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

        var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

        await customerService.AddCartItemAsync(productId, quantity, CancellationToken.None);

        // Act
        var result = await customerService.RemoveCartItemAsync(productId, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        _factory.DbContext.ChangeTracker.Clear();
        var customer = await _factory.DbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

        customer!.Cart!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveCartItemAsync_WithNonExistentItem_ShouldReturnCartItemNotFoundError()
    {
        // Arrange
        var productId = Constants.Product.Id;

        var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

        // Act
        var result = await customerService.RemoveCartItemAsync(productId, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Contain("CartItem.NotFound");
    }

    [Fact]
    public async Task RemoveCartItemAsync_WithMultipleItems_ShouldOnlyRemoveSpecifiedItem()
    {
        // Arrange
        var firstProductId = Constants.Product.Id;
        var secondProductId = Constants.Product.AlternativeId;
        var quantity = Constants.Product.DefaultQuantity;
        var merchantId = Constants.Merchant.Id;

        var firstProductResponse = new ProductResponse(
            firstProductId,
            Constants.Product.Title,
            merchantId,
            Constants.Product.BasePrice);

        var secondProductResponse = new ProductResponse(
            secondProductId,
            "Another Product",
            merchantId,
            Constants.Product.BasePrice);

        _factory.SetupProductQuery(firstProductId, firstProductResponse);
        _factory.SetupProductQuery(secondProductId, secondProductResponse);

        var (customerService, repository) = await TestHelper.CreateCustomerServiceAsync(_factory);

        await customerService.AddCartItemAsync(firstProductId, quantity, CancellationToken.None);
        await customerService.AddCartItemAsync(secondProductId, quantity, CancellationToken.None);

        // Act
        var result = await customerService.RemoveCartItemAsync(firstProductId, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        var customer = await _factory.DbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

        customer!.Cart!.Items.Should().HaveCount(1);
        customer.Cart.Items.First().ProductId.Should().Be(secondProductId);
    }
}
