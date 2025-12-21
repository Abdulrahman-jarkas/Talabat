using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Users.Domain.MerchantAggregate;

internal class Merchant : Entity
{
	public string Email { get; private set; } = string.Empty;

	internal Merchant(string email, Guid? id = null) : base(id ?? Guid.NewGuid())
	{
		Email = Guard.Against.NullOrEmpty(email, nameof(email));
	}

	private Merchant()
	{
		// EF 
	}
}