using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.Domain.CustomerAggregate.Events;
using CartEntity = Talabat.Users.Domain.CustomerAggregate.Cart.Cart;

namespace Talabat.Users.Domain.CustomerAggregate;

internal class Customer : AggregateRoot
{
	public string Email { get; private set; } = string.Empty;

	public CartEntity? Cart { get; set; } = null;

	private readonly List<CustomerAddress> _addresses = new();
	public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

	internal Customer(Guid id, string email) : base(id)
	{
		Guard.Against.Default(id, nameof(id));
		Email = Guard.Against.NullOrEmpty(email, nameof(email));
	}

	public ErrorOr<Success> ResetCart(bool raiseEvent = true)
	{
		Cart = null;

		if (raiseEvent)
			_domainEvents.Add(new CartChangedEvent(Id));

		return Result.Success;
	}

	public ErrorOr<Updated> SetCartItem(Guid shopId, Guid productId, int quantity)
	{
		if (Cart is null)
			Cart = new CartEntity(shopId);

		if (Cart.ShopId != shopId)
			return CustomerErrors.ShopMismatch;

		var result = Cart.SetCartItem(productId, quantity);
		if (result.IsError)
			return result.Errors;

		Cart = result.Value;
		_domainEvents.Add(new CartChangedEvent(Id));
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
		_domainEvents.Add(new CartChangedEvent(Id));
		return Result.Updated;
	}

	public void AddAddress(string address)
	{
		var customerAddress = new CustomerAddress(address);
		_addresses.Add(customerAddress);
	}

	public ErrorOr<Success> RemoveAddress(Guid addressId)
	{
		var address = _addresses.FirstOrDefault(a => a.Id == addressId);
		if (address is null)
			return Error.NotFound("Customer.AddressNotFound", "Address not found.");

		_addresses.Remove(address);
		return Result.Success;
	}

	private Customer()
	{
		// EF 
	}
}