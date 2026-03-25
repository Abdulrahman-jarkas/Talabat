using Microsoft.EntityFrameworkCore;
using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Repositories;

internal class UsersRepository(UsersDbContext dbContext) : IUsersRepository
{
	public Task<Customer?> GetCustomerAsync(Guid customerId)
	{
		return dbContext.Customers
			.Include(c => c.ActiveCheckoutSession)
				.ThenInclude(cs => cs!.Items)
			.FirstOrDefaultAsync(c => c.Id == customerId);
	}

	public Task<Customer?> GetCustomerDetailsAsync(Guid customerId, CancellationToken cancellationToken)
	{
		return dbContext.Customers
			.Include(c => c.Addresses)
			.Include(c => c.ActiveCheckoutSession)
				.ThenInclude(cs => cs!.Items)
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
	}

	public Task<List<Customer>?> GetCustomersWithProductInCartAsync(Guid productId, CancellationToken cancellationToken = default)
	{
		return dbContext.Customers
			.Where(c => c.Cart != null && c.Cart.Items.Any(i => i.ProductId == productId))
			.ToListAsync(cancellationToken)!;
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		// For JSON columns (like Cart), EF Core might not detect changes automatically
		// Mark Cart as modified for all tracked Customer entities
		//var customerEntries = dbContext.ChangeTracker.Entries<Customer>()
		//	.Where(e => e.State == EntityState.Modified);

		//foreach (var entry in customerEntries)
		//{
		//	entry.Property(c => c.Cart).IsModified = true;
		//}

		return dbContext.SaveChangesAsync(cancellationToken);
	}
}