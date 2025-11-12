using Microsoft.EntityFrameworkCore;

namespace Talabat.Taxes;

public class TaxesRepository(TaxesDbContext dbContext) : ITaxesRepository
{
	public async Task AddTaxPolicy(TaxPolicy taxPolicy, CancellationToken cancellationToken = default)
	{
		 await dbContext.TaxPolicies.AddAsync(taxPolicy, cancellationToken);
	}

	public void UpdateTaxPolicy(TaxPolicy taxPolicy, CancellationToken cancellationToken = default)
	{
		 dbContext.Update(taxPolicy);
	}

	public async Task AddTaxCategory(TaxCategory category, CancellationToken cancellationToken = default)
	{
		await dbContext.TaxCategories.AddAsync(category, cancellationToken);
	}

	public Task<TaxPolicy?> GetTaxPolicy(string countryCode)
	{
		return dbContext.TaxPolicies
			.Include(t => t.Categories)
			.FirstOrDefaultAsync(x => x.CountryCode == countryCode);
	}

	public Task SaveChangesAsync()
	{
		return dbContext.SaveChangesAsync();
	}
}
