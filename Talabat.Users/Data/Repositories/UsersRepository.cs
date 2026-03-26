using Microsoft.EntityFrameworkCore;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users.Data.Repositories;

internal class UsersRepository(UsersDbContext dbContext) : IUsersRepository
{
	public async Task<Customer?> GetCustomerWithActiveCheckoutAsync(
		Guid customerId, 
		CancellationToken cancellationToken = default)
	{
		return await dbContext.Customers
			.Include(c => c.CheckoutSessions.Where(cs => cs.Status == CheckoutSessionStatus.Active))
				.ThenInclude(cs => cs.Items)
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
	}

	public async Task<Customer?> GetCustomerWithAddressesAsync(
		Guid customerId, 
		CancellationToken cancellationToken = default)
	{
		return await dbContext.Customers
			.Include(c => c.Addresses)
			.Include(c => c.CheckoutSessions.Where(cs => cs.Status == CheckoutSessionStatus.Active))
				.ThenInclude(cs => cs.Items)
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
	}

	public async Task<Customer?> GetCustomerByCheckoutSessionIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken)
	{
		return await dbContext.Customers
			.Include(c => c.CheckoutSessions)
				.ThenInclude(cs => cs.Items)
			.FirstOrDefaultAsync(c => c.CheckoutSessions.Any(cs => cs.Id == checkoutSessionId), cancellationToken);
	}

	public async Task<List<Customer>?> GetCustomersWithProductInCartAsync(Guid productId, CancellationToken cancellationToken = default)
	{
		// Load all customers with carts (Cart is JSON column, can't be queried directly in LINQ)
		var customersWithCart = await dbContext.Customers
			.Where(c => c.Cart != null)
			.Include(c => c.CheckoutSessions)
				.ThenInclude(cs => cs.Items)
			.ToListAsync(cancellationToken);

		// Filter in memory since Cart.Items is a JSON column
		return customersWithCart
			.Where(c => c.Cart!.Items.Any(i => i.ProductId == productId))
			.ToList();
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return dbContext.SaveChangesAsync(cancellationToken);
	}
}