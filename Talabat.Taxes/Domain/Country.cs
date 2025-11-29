using Talabat.Taxes.Common;

namespace Talabat.Taxes.Domain;

public class Country : AggregateRoot
{
	public string Code { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Currency { get; set; } = string.Empty;

	public ServiceFees ServiceFees { get; set; }

	public Country(string code, string name, string currency, ServiceFees serviceFees)
	{
		Code = code;
		Name = name;
		Currency = currency;
		ServiceFees = serviceFees;
	}

	public Country()
	{
	}
}
