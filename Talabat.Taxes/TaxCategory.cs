using Talabat.Taxes.Common;

namespace Talabat.Taxes;

public class TaxCategory : Entity
{
	public string Name { get; private set; } = string.Empty;
	public decimal VatPercentage { get; private set; }

	public TaxCategory(string name, decimal vat)
	{
		if(string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Tax category name cannot be null or empty.", nameof(name));

		if (vat < 0 || vat > 100)
			throw new ArgumentOutOfRangeException(nameof(vat), "VAT percentage must be between 0 and 100.");

		Name = name;
		VatPercentage = vat;
	}

	public TaxCategory()
	{
	}
}