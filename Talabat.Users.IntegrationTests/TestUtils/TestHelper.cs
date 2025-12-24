using Microsoft.EntityFrameworkCore;
using Talabat.Products.Contracts;
using Talabat.Users.Data.Repositories;
using Talabat.Users.IntegrationTests.Infrastructure;
using Talabat.Users.IntegrationTests.TestConstants;

namespace Talabat.Users.IntegrationTests.TestUtils;

internal static class TestHelper
{
    internal static async Task<(Users.CustomerService service, UsersRepository repository)> CreateCustomerServiceAsync(
        UsersApiFactory factory)
    {
        var repository = new UsersRepository(factory.DbContext);
        var service = new Users.CustomerService(factory.MockMediator, repository);
        return (service, repository);
    }

    internal static async Task SetupCartWithProductAsync(
        UsersApiFactory factory,
        Users.CustomerService customerService,
        Guid? productId = null,
        Guid? merchantId = null,
        int? quantity = null,
        decimal? price = null)
    {
        var prodId = productId ?? Constants.Product.Id;
        var merchId = merchantId ?? Constants.Merchant.Id;
        var qty = quantity ?? Constants.Product.DefaultQuantity;
        var basePrice = price ?? Constants.Product.BasePrice;

        var productResponse = new ProductResponse(
            prodId,
            Constants.Product.Title,
            merchId,
            basePrice);

        factory.SetupProductQuery(prodId, productResponse);
        await customerService.AddCartItemAsync(prodId, qty, CancellationToken.None);
    }

    internal static async Task<Guid> CreateAndAddAddressAsync(
        UsersApiFactory factory,
        string? address = null)
    {
        var customer = await factory.DbContext.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == Constants.Customer.Id);

        var addressValue = address ?? Constants.Address.DefaultAddress;
        customer!.AddAddress(addressValue);
        await factory.DbContext.SaveChangesAsync();

        return customer.Addresses.Last().Id;
    }
}
