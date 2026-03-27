using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using CartEntity = Talabat.Users.Domain.CustomerAggregate.Cart.Cart;

namespace Talabat.Users.Domain.CustomerAggregate;

internal class Customer : AggregateRoot
{
	public string Email { get; private set; } = string.Empty;

	public CartEntity? Cart { get; set; } = null;

	private readonly List<CustomerAddress> _addresses = new();
	public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

	internal Customer(string email, Guid? id = null) : base(id ?? Guid.NewGuid())
	{
		Email = Guard.Against.NullOrEmpty(email, nameof(email));
	}

	public ErrorOr<Success> ResetCart()
	{
		Cart = null;

		return Result.Success;
	}

	public ErrorOr<Updated> SetCartItem(Guid productOwner, Guid productId, int quantity)
	{
		if (Cart is null)
			Cart = new CartEntity(productOwner);

		if (Cart.MerchantId != productOwner)
			return CustomerErrors.MerchantMismatch;

		var result = Cart.SetCartItem(productId, quantity);
		if (result.IsError)
			return result.Errors;

		Cart = result.Value;
		return Result.Updated;
	}

	public ErrorOr<Updated> RemoveCartItem(Guid productId)
	{
		if (Cart is null)
			return CustomerErrors.CartNotFound;

		var result = Cart.RemoveCartItem(productId);
		if (result.IsError)
			return result.Errors;

		Cart = result.Value;
		return Result.Updated;
	}

	public void AddAddress(string address)
	{
		var customerAddress = new CustomerAddress(address);
		_addresses.Add(customerAddress);
	}

	private Customer()
	{
		// EF 
	}
}