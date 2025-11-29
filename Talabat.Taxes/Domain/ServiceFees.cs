using Talabat.Taxes.Common;

namespace Talabat.Taxes.Domain;

public class ServiceFees : ValueObject
{
	public decimal Value { get; private set; }
	public int TaxId { get; set; }

	public ServiceFees(decimal value, int taxId)
	{
		Value = value;
		TaxId = taxId;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		return new object[]
		{
			Value,
			TaxId
		};
	}
}