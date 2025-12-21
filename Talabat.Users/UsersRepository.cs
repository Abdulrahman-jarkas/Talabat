using Microsoft.EntityFrameworkCore;

namespace Talabat.Users;

internal class UsersRepository(UsersDbContext dbContext) : IUsersRepository
{
	public Task<Customer?> GetCustomerAsync(Guid customerId)
	{
		return dbContext.Customers
			.FirstOrDefaultAsync(c => c.Id == customerId);
	}

	public Task<Customer?> GetCustomerDetailsAsync(Guid customerId, CancellationToken cancellationToken)
	{
		return dbContext.Customers
			.Include(c => c.ActiveCheckoutSession)
			.Include(c => c.Addresses)
			.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return dbContext.SaveChangesAsync(cancellationToken);
	}
}