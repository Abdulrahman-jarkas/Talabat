namespace Talabat.Taxes;

public class TaxPolicy
{
	private readonly List<TaxCategory> _categories = new List<TaxCategory>();
	public IReadOnlyList<TaxCategory> Categories => _categories.AsReadOnly();

	public string CountryCode { get; private set; } = string.Empty;

	public TaxCategoryCodeEnum ServiceFeeTaxCategory { get; private set; }

	public TaxPolicy(
		string countryCode,
		IEnumerable<TaxCategory> categories,
		TaxCategoryCodeEnum serviceFeeTaxCategory)
	{
		CountryCode = countryCode;
		_categories.AddRange(categories);
		ServiceFeeTaxCategory = serviceFeeTaxCategory;
	}

	public TaxCategory ServiceFeeTax => _categories.First(c => c.Code == ServiceFeeTaxCategory);
}

public static class CountryCodes
{
	public const string UAE = "AE";
	public const string KSA = "SA";
}

public static class TaxPolicies
{
	public static readonly TaxPolicy UAE = new TaxPolicy(
		countryCode: CountryCodes.KSA,
		categories: new[]
		{
			new TaxCategory("Standard VAT", 5, TaxCategoryCodeEnum.Standard),
			new TaxCategory("Zero Rated", 0, TaxCategoryCodeEnum.ZeroRated),
			new TaxCategory("Exempt", 0, TaxCategoryCodeEnum.Exempt)
		},
		TaxCategoryCodeEnum.Standard
	);

	public static readonly TaxPolicy KSA = new TaxPolicy(
		countryCode: CountryCodes.KSA,
		categories: new[]
		{
			new TaxCategory("Standard VAT", 15, TaxCategoryCodeEnum.Standard),
			new TaxCategory("Zero Rated", 0, TaxCategoryCodeEnum.ZeroRated),
			new TaxCategory("Exempt", 0, TaxCategoryCodeEnum.Exempt)
		},
		TaxCategoryCodeEnum.Standard
	);
}
