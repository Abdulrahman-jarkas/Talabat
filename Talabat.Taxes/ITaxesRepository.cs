
namespace Talabat.Taxes
{
	public interface ITaxesRepository
	{
		Task AddTaxCategory(TaxCategory category, CancellationToken cancellationToken = default);
		Task AddTaxPolicy(TaxPolicy taxPolicy, CancellationToken cancellationToken = default);
		Task<TaxPolicy?> GetTaxPolicy(string countryCode);
		Task SaveChangesAsync();
	}
}