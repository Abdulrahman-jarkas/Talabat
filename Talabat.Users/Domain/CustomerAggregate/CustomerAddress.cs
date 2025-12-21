using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.CustomerAggregate;

internal class CustomerAddress : Entity
{
	public string Address { get; set; } = string.Empty;

	internal CustomerAddress(string address, Guid? id = null) : base(id ?? Guid.NewGuid())
	{
		Address = Guard.Against.NullOrEmpty(address, nameof(address));
	}
}