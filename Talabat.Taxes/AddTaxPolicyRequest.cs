namespace Talabat.Taxes;

public sealed class AddTaxPolicyRequest
{
	public string CountryCode { get; init; } = string.Empty;

	public List<TaxCategoryDto> Categories { get; init; } = new();
}

public sealed class TaxCategoryDto
{
	public string Name { get; init; } = string.Empty;
	public decimal VatPercentage { get; init; }
}