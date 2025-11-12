using ErrorOr;

namespace Talabat.Taxes;

public class TaxesService(ITaxesRepository taxesRepository) : ITaxesService
{
	public async Task<ErrorOr<Success>> AddTaxPolicy(AddTaxPolicyRequest request, CancellationToken cancellationToken = default)
	{
		var policy = new TaxPolicy(request.CountryCode, []);


		foreach (var item in request.Categories)
		{
			var cat = new TaxCategory(item.Name, item.VatPercentage);
			policy.AddTaxCategory(cat);
		}

		await taxesRepository.AddTaxPolicy(policy, cancellationToken);
		await taxesRepository.SaveChangesAsync();

		return Result.Success;
	}

	public Task<TaxPolicy?> GetTaxPolicy(string countryCode)
	{
		return taxesRepository.GetTaxPolicy(countryCode);
	}
}