using Microsoft.EntityFrameworkCore;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users.Data.Repositories;

internal class UsersRepository(UsersDbContext dbContext) : IUsersRepository
{
	public async Task<Customer?> GetCustomerAsync(Guid customerId)
	{
		return await dbContext.Customers
			.Include(c => c.CheckoutSessions.Where(cs => cs.Status == CheckoutSessionStatus.Active))
				.ThenInclude(cs => cs.Items)
			.FirstOrDefaultAsync(c => c.Id == customerId);
	}

	public async Task<Customer?> GetCustomerDetailsAsync(Guid customerId, CancellationToken cancellationToken)
	{
		return await dbContext.Customers
			.Include(c => c.Addresses)
			.Include(c => c.CheckoutSessions.Where(cs => cs.Status == CheckoutSessionStatus.Active))
				.ThenInclude(cs => cs.Items)
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
		return dbContext.SaveChangesAsync(cancellationToken);
	}
}