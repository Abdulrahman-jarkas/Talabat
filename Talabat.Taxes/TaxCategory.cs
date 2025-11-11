namespace Talabat.Taxes;

public class TaxCategory
{
	public string Name { get; private set; } = string.Empty;
	public decimal VatPercentage { get; private set; }
	public TaxCategoryCodeEnum Code { get; init; }

	public TaxCategory(string name, decimal vat, TaxCategoryCodeEnum code)
	{
		Name = name;
		VatPercentage = vat;
		Code = code;
	}
}

public enum TaxCategoryCodeEnum
{
	Standard,
	ZeroRated,
	Exempt
}