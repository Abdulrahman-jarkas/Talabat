using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using CartEntity = Talabat.Users.Domain.CustomerAggregate.Cart.Cart;

namespace Talabat.Users.Domain.CustomerAggregate;

internal class Customer : AggregateRoot
{
	public string Email { get; private set; } = string.Empty;

	public CartEntity? Cart { get; private set; } = null;

	private readonly List<CustomerAddress> _addresses = new();
	public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

	public CheckoutSession? ActiveCheckoutSession { get; private set; } = null;

	internal Customer(string email, Guid? id = null) : base(id ?? Guid.NewGuid())
	{
		Email = Guard.Against.NullOrEmpty(email, nameof(email));
	}

	public ErrorOr<Success> ResetCart()
	{
		if (ActiveCheckoutSession is not null)
			return CustomerErrors.UpdateCartWithActiveCheckoutSession;

		Cart = null;

		return Result.Success;
	}

	public ErrorOr<Updated> SetCartItem(Guid productOwner, Guid productId, int quantity)
	{
		if (ActiveCheckoutSession is not null)
			return CustomerErrors.UpdateCartWithActiveCheckoutSession;

		if (Cart is null)
			Cart = new CartEntity(productOwner);

		if (Cart.MerchantId != productOwner)
			return CustomerErrors.MerchantMismatch;

		return Cart.SetCartItem(productId, quantity);
	}

	public ErrorOr<Updated> RemoveCartItem(Guid productId)
	{
		if (Cart is null)
			return CustomerErrors.CartNotFound;

		return Cart.RemoveCartItem(productId);
	}

	public ErrorOr<Created> CreateCheckoutSession(CheckoutSession checkoutSession)
	{
		if (Cart is null || !Cart.Items.Any())
			return CustomerErrors.CartNotFound;

		if (ActiveCheckoutSession is not null)
			return CustomerErrors.ActiveCheckoutSessionExists;

		ActiveCheckoutSession = checkoutSession;

		return Result.Created;
	}

	public ErrorOr<Success> CancelCheckoutSession()
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		ActiveCheckoutSession = null;

		return Result.Success;
	}

	public void ResetActiveCheckoutSession()
	{
		ActiveCheckoutSession = null;
	}

	public ErrorOr<Success> SetAddressForOrder(Guid addressId)
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		var addressExists = _addresses.Any(a => a.Id == addressId);
		if (!addressExists)
			return CustomerErrors.AddressNotFound;

		ActiveCheckoutSession.SetAddress(addressId);
		return Result.Success;
	}

	public void AddAddress(string address)
	{
		var customerAddress = new CustomerAddress(address);
		_addresses.Add(customerAddress);
	}

	public void ResetCartAndCheckoutSession()
	{
		Cart = null;
		ActiveCheckoutSession = null;
	}

	private Customer()
	{
		// EF 
	}
}