using ErrorOr;

namespace Talabat.Taxes
{
	public interface ITaxesService
	{
		Task<ErrorOr<Success>> AddTaxPolicy(AddTaxPolicyRequest request, CancellationToken cancellationToken = default);
		Task<TaxPolicy?> GetTaxPolicy(string countryCode);
	}
}