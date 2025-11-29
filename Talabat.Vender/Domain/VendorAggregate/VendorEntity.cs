using Talabat.Vender.Core.Common;

namespace Talabat.Vender.Domain.VendorAggregate;

public class VendorEntity : AggregateRoot
{
	public string Name { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;
	public int CountryId { get; set; }

	public VendorEntity(string name, string email, int countryId, int id) : base(id)
	{
		//TODO: Add Email and Name validation
		Name = name;
		Email = email;
		CountryId = countryId;
	}
}
