using ErrorOr;
using MediatR;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.Users.Application.Services;
using Talabat.Users.Data.Repositories;
using Talabat.Users.Domain.CustomerAggregate;
using Talabat.Users.Domain.CustomerAggregate.Cart;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users;

internal class CustomerService : ICustomerService
{
	private readonly Guid _customerId;
	private readonly ISender _sender;
	private readonly IUsersRepository _usersRepository;
	private readonly IProductService _productService;

	public CustomerService(
		Guid customerId,
		ISender sender,
		IUsersRepository usersRepository,
		IProductService productService)
	{
		_customerId = customerId;
		_sender = sender;
		_usersRepository = usersRepository;
		_productService = productService;
	}

	public async Task<ErrorOr<Success>> AddCartItemAsync(Guid productId, int quantity, CancellationToken cancellationToken)
	{
		var customer = await _usersRepository.GetCustomerAsync(_customerId);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var product = await _sender.Send(new ProductQuery(productId), cancellationToken);
		if (product is null)
			return CartErrors.NoProductFoundForCartItem(productId);

		var setCartResult = customer.SetCartItem(product.Merchant, productId, quantity);
		if (setCartResult.IsError)
			return setCartResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> RemoveCartItemAsync(Guid productId, CancellationToken cancellationToken)
	{
		var customer = await _usersRepository.GetCustomerAsync(_customerId);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var setCartResult = customer.RemoveCartItem(productId);
		if (setCartResult.IsError)
			return setCartResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> CreateCheckoutSession(CancellationToken cancellationToken = default)
	{
		var customer = await _usersRepository.GetCustomerAsync(_customerId);

		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var createResult = await customer.CreateCheckoutSession(_productService, cancellationToken);
		if (createResult.IsError)
			return createResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<Success>> CancelCheckoutSession(CancellationToken cancellationToken = default)
	{
		var customer = await _usersRepository.GetCustomerAsync(_customerId);

		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var cancelResult = customer.CancelCheckoutSession();

		if (cancelResult.IsError)
			return cancelResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> Checkout(Guid addressId, PaymentType paymentType, CancellationToken cancellationToken = default)
	{
		var customer = await _usersRepository.GetCustomerDetailsAsync(_customerId, cancellationToken);

		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		if (customer.Cart is null || customer.Cart.Items.Any() == false)
			return CartErrors.CartNotFound;

		if (customer.ActiveCheckoutSession is null)
			return CustomerErrors.NoActiveCheckoutSession;

		var setAddressResult = customer.SetAddressForOrder(addressId);

		if (setAddressResult.IsError)
			return setAddressResult.Errors;

		var setPaymentTypeResult = customer.SetPaymentType(paymentType);

		if (setPaymentTypeResult.IsError)
			return setPaymentTypeResult.Errors;

		var productsIds = customer.Cart.Items.Select(i => i.ProductId).ToList();
		var products = await _sender.Send(new ProductsQuery(productsIds), cancellationToken);

		foreach (var item in customer.ActiveCheckoutSession.Items)
		{
			var product = products?.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null)
				return CartErrors.NoProductFoundForCartItem(item.ProductId);

			if (product.BasePrice != item.BasePrice)
				return CheckoutSessionErrors.PriceMismatch;
		}

		var paymentSession = await _sender.Send(
			new CreatePaymentSessionRequest(
				_customerId,
				customer.ActiveCheckoutSession.Id,
				customer.ActiveCheckoutSession.TotalPrice),
			cancellationToken);

		if (paymentSession.IsError)
			return paymentSession.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return (paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}
}