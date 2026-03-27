using Microsoft.EntityFrameworkCore;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Repositories;

internal class UsersRepository(UsersDbContext dbContext) : IUsersRepository
{
	public async Task<Customer?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
	{
		return await dbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
	}

	public async Task<List<Customer>?> GetCustomersWithProductInCartAsync(Guid productId, CancellationToken cancellationToken = default)
	{
		// Load all customers with carts (Cart is JSON column, can't be queried directly in LINQ)
		var customersWithCart = await dbContext.Customers
			.Where(c => c.Cart != null)
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