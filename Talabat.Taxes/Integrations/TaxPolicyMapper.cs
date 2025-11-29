using Talabat.Taxes.Contracts;
using Talabat.Taxes.Domain;

namespace Talabat.Taxes.Integrations;

public static class TaxPolicyMapper
{
	public static TaxResponse ToResponse(this Tax policy)
	{
		if (policy is null)
			throw new ArgumentNullException(nameof(policy));

		return new TaxResponse(
			Id: policy.Id,
			Name: policy.Name,
			VatPercentage: policy.VatPercentage
		);
	}
}
