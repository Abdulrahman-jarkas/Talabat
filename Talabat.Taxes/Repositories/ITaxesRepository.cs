using Talabat.Taxes.Domain;

namespace Talabat.Taxes.Repositories
{
	public interface ITaxesRepository
	{
		Task<Tax?> GetTax(int id, CancellationToken cancellationToken = default);
		Task<Country?> GetCountry(int id, CancellationToken cancellationToken = default);
	}
}