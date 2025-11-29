using Microsoft.EntityFrameworkCore;
using Talabat.Taxes.Configuration;
using Talabat.Taxes.Domain;

namespace Talabat.Taxes.Repositories;

public class TaxesRepository(TaxesDbContext dbContext) : ITaxesRepository
{
	public Task<Country?> GetCountry(int id, CancellationToken cancellationToken = default)
	{
		return dbContext.Countries.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
	}

	public Task<Tax?> GetTax(int id, CancellationToken cancellationToken = default)
	{
		return dbContext.Taxes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
	}
}
