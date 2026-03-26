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
		var customer = await _usersRepository.GetCustomerWithActiveCheckoutAsync(_customerId, cancellationToken);
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
		var customer = await _usersRepository.GetCustomerWithActiveCheckoutAsync(_customerId, cancellationToken);
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
		var customer = await _usersRepository.GetCustomerWithActiveCheckoutAsync(_customerId, cancellationToken);

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
		var customer = await _usersRepository.GetCustomerWithActiveCheckoutAsync(_customerId, cancellationToken);

		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		var cancelResult = customer.CancelCheckoutSession();

		if (cancelResult.IsError)
			return cancelResult.Errors;

		await _usersRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> Checkout(Guid addressId, CancellationToken cancellationToken = default)
	{
		// 1. Load and prepare checkout
		var prepareResult = await PrepareCheckoutAsync(addressId, cancellationToken);
		if (prepareResult.IsError)
			return prepareResult.Errors;

		var customer = prepareResult.Value;

		// 2. Create payment session (infrastructure/integration concern)
		var paymentSession = await _sender.Send(
			new CreatePaymentSessionRequest(
				_customerId,
				customer.ActiveCheckoutSession!.Id,
				customer.ActiveCheckoutSession.TotalPrice),
			cancellationToken);

		if (paymentSession.IsError)
			return paymentSession.Errors;

		// 3. Store payment reference (domain operation)
		var setPaymentIdResult = customer.SetPaymentIdForActiveCheckoutSession(paymentSession.Value.PaymentId);
		if (setPaymentIdResult.IsError)
			return setPaymentIdResult.Errors;

		// 4. Persist changes (completion happens via PaymentSuccessedEvent)
		await _usersRepository.SaveChangesAsync(cancellationToken);

		return (paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}

	private async Task<ErrorOr<Customer>> PrepareCheckoutAsync(Guid addressId, CancellationToken cancellationToken)
	{
		// 1. Load customer aggregate
		var customer = await _usersRepository.GetCustomerWithAddressesAsync(_customerId, cancellationToken);
		if (customer is null)
			return CustomerErrors.CustomerNotFound;

		// 2. Prepare checkout (domain business logic encapsulated)
		var prepareResult = customer.PrepareForCheckout(addressId);
		if (prepareResult.IsError)
			return prepareResult.Errors;

		// 3. Validate prices with external product service (infrastructure concern)
		var productIds = customer.Cart!.Items.Select(i => i.ProductId).ToList();
		var products = await _sender.Send(new ProductsQuery(productIds), cancellationToken);

		var currentPrices = products?.Select(p => (p.Id, p.BasePrice)) ?? Enumerable.Empty<(Guid, decimal)>();
		var priceValidationResult = customer.ActiveCheckoutSession!.ValidatePrices(currentPrices);
		if (priceValidationResult.IsError)
			return priceValidationResult.Errors;

		return customer;
	}
}
