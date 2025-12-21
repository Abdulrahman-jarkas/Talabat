using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Talabat.Invoices;

internal class InvoicesRepository : IInvoicesRepository
{
	private readonly InvoicesDbContext _dbContext;

	public InvoicesRepository(InvoicesDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public ValueTask<EntityEntry<Invoice>> AddAsync(Invoice invoice)
	{
		return _dbContext.Invoices.AddAsync(invoice);
	}

	public Task<Customer?> GetCustomerAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return _dbContext.Customers
			.Where(c => c.Id == userId)
			.Include(c => c.Invoices)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public Task SaveChangesAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}