using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;
using Talabat.Users.Application.Services;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.Domain.CustomerAggregate.Checkout;
using CartEntity = Talabat.Users.Domain.CustomerAggregate.Cart.Cart;

namespace Talabat.Users.Domain.CustomerAggregate;

internal class Customer : AggregateRoot
{
	public string Email { get; private set; } = string.Empty;

	public CartEntity? Cart { get; set; } = null;

	private readonly List<CustomerAddress> _addresses = new();
	public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

	private readonly List<CheckoutSession> _checkoutSessions = new();
	public IReadOnlyCollection<CheckoutSession> CheckoutSessions => _checkoutSessions.AsReadOnly();

	public CheckoutSession? ActiveCheckoutSession => _checkoutSessions.FirstOrDefault(cs => cs.Status == CheckoutSessionStatus.Active);

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

	public async Task<ErrorOr<Created>> CreateCheckoutSession(IProductService productService, CancellationToken cancellationToken = default)
	{
		if (Cart is null || !Cart.Items.Any())
			return CustomerErrors.CartNotFound;

		if (ActiveCheckoutSession is not null)
			return CustomerErrors.ActiveCheckoutSessionExists;

		var productIds = Cart.Items.Select(i => i.ProductId).ToList();
		var productsResult = await productService.GetProductsDetailsAsync(productIds, cancellationToken);

		if (productsResult is null || !productsResult.Any())
			return CartErrors.NoProductsFoundForCartItems;

		var checkoutItems = new List<CheckoutItem>();
		foreach (var cartItem in Cart.Items)
		{
			var product = productsResult.FirstOrDefault(p => p.Id == cartItem.ProductId);
			if (product is null)
				return CartErrors.NoProductFoundForCartItem(cartItem.ProductId);

			checkoutItems.Add(CheckoutItem.Create(
				cartItem.ProductId,
				cartItem.Quantity,
				product.BasePrice));
		}

		var checkoutSession = CheckoutSession.Create(Cart.MerchantId, checkoutItems);
		_checkoutSessions.Add(checkoutSession);

		return Result.Created;
	}

	public ErrorOr<Success> CancelCheckoutSession()
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		_checkoutSessions.Remove(ActiveCheckoutSession);

		return Result.Success;
	}

	public void ResetActiveCheckoutSession()
	{
		if (ActiveCheckoutSession is not null)
			_checkoutSessions.Remove(ActiveCheckoutSession);
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

	public ErrorOr<Success> SetPaymentType(PaymentType paymentType)
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		ActiveCheckoutSession.SetPaymentType(paymentType);
		return Result.Success;
	}

	public ErrorOr<Success> SetPaymentId(Guid paymentId)
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		ActiveCheckoutSession.SetPaymentId(paymentId);
		return Result.Success;
	}

	public ErrorOr<Success> SetOrderId(Guid orderId)
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		ActiveCheckoutSession.SetOrderId(orderId);
		return Result.Success;
	}

	public ErrorOr<Success> CompleteCheckoutSession()
	{
		if (ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		ActiveCheckoutSession.Complete();
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
		if (ActiveCheckoutSession is not null)
			_checkoutSessions.Remove(ActiveCheckoutSession);
	}

	private Customer()
	{
		// EF 
	}
}