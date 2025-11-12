using Talabat.Taxes.Contracts;

namespace Talabat.Taxes.Integrations;

public static class TaxPolicyMapper
{
	public static TaxPolicyResponse ToResponse(this TaxPolicy policy)
	{
		if (policy is null)
			throw new ArgumentNullException(nameof(policy));

		return new TaxPolicyResponse(
			Id: policy.Id,
			CountryCode: policy.CountryCode,
			Categories: policy.Categories
				.Select(c => c.ToResponse())
				.ToList()
		);
	}

	public static TaxCategoryResponse ToResponse(this TaxCategory category)
	{
		if (category is null)
			throw new ArgumentNullException(nameof(category));

		return new TaxCategoryResponse(
			Id: category.Id,
			Name: category.Name,
			vat: category.VatPercentage
		);
	}
}
