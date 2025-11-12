using Talabat.Taxes.Common;
using ErrorOr;

namespace Talabat.Taxes;

public class TaxPolicy : AggregateRoot
{
	private readonly List<TaxCategory> _categories = new List<TaxCategory>();
	public IReadOnlyList<TaxCategory> Categories => _categories.AsReadOnly();

	public string CountryCode { get; init; }

	public TaxPolicy(
		string countryCode,
		IEnumerable<TaxCategory> categories)
	{
		if (CountryCodes.GetCountryName(countryCode).IsError)
			throw new ArgumentException("Invalid country code", nameof(countryCode));

		CountryCode = countryCode;

		foreach (var category in categories)
		{
			AddTaxCategory(category);
		}
	}

	public void AddTaxCategory(TaxCategory taxCategory)
	{
		_categories.Add(taxCategory);
	}

	public TaxPolicy()
	{
	}
}