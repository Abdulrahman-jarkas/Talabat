using ErrorOr;

namespace Talabat.Taxes.Common;

public static class CountryCodes
{
	public const string UAE = "AE";
	public const string KSA = "SA";

	public static ErrorOr<string> GetCountryName(string countryCode)
	{
		return countryCode switch
		{
			UAE => "United Arab Emirates",
			KSA => "Kingdom of Saudi Arabia",
			_ => Error.Validation(
				code: "InvalidCountryCode",
				description: $"The country code '{countryCode}' is not recognized."
			)
		};
	}
}