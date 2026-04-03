using ErrorOr;
using MediatR;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;
using Talabat.Users.Contracts;

namespace Talabat.Checkout.Application.Services;

internal class CheckoutSessionService : ICheckoutSessionService
{
	private readonly ISender _sender;
	private readonly ICheckoutSessionRepository _checkoutSessionRepository;
	private readonly IProductRepository _productRepository;
	private readonly IProductService _productService;

	public CheckoutSessionService(
		ISender sender,
		ICheckoutSessionRepository checkoutSessionRepository,
		IProductRepository productRepository,
		IProductService productService)
	{
		_sender = sender;
		_checkoutSessionRepository = checkoutSessionRepository;
		_productRepository = productRepository;
		_productService = productService;
	}

	public async Task<ErrorOr<Success>> CreateAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
	{
		// 0. Check for any existing active session for this customer
		var existingSession = await _checkoutSessionRepository.GetActiveByCustomerIdAsync(customerId, cancellationToken);
		if (existingSession is not null && existingSession.Lifetime.IsActive)
			return CheckoutSessionErrors.ActiveSessionAlreadyExists;

		// 1. Get cart details from Users context
		var cartDetails = await _sender.Send(new CartDetailsQuery(customerId), cancellationToken);

		if (cartDetails is null || cartDetails.CartItems.Count == 0)
			return CheckoutSessionErrors.CartEmpty;

		// 2. Get latest product details from Products module
		var productIds = cartDetails.CartItems.Select(ci => ci.productId).ToList();
		var productDetails = await _productService.GetProductDetailsAsync(productIds, cancellationToken);

		if (productDetails is null || productDetails.Count == 0)
			return CheckoutSessionErrors.ProductsNotFound;

		// 3. Sync Checkout Products (create or update local projections)
		var checkoutProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

		foreach (var detail in productDetails)
		{
			var checkoutProduct = checkoutProducts.FirstOrDefault(p => p.Id == detail.ProductId);
			if (checkoutProduct is null)
			{
				checkoutProduct = new Domain.ProductAggregate.Product(detail.ProductId, detail.Quantity, detail.BasePrice);
				await _productRepository.AddAsync(checkoutProduct, cancellationToken);
				checkoutProducts.Add(checkoutProduct);
			}
			else
				{
					var quantityResult = checkoutProduct.UpdateQuantity(detail.Quantity);
					if (quantityResult.IsError)
						return quantityResult.Errors;

					var priceResult = checkoutProduct.UpdatePrice(detail.BasePrice);
					if (priceResult.IsError)
						return priceResult.Errors;
				}
		}

		// 4. Build checkout items, validate availability, and reserve on Checkout Products
		var checkoutItems = new List<CheckoutItem>();
		foreach (var cartItem in cartDetails.CartItems)
		{
			var product = checkoutProducts.FirstOrDefault(p => p.Id == cartItem.productId);
			if (product is null)
				return CheckoutSessionErrors.ProductNotFound(cartItem.productId);

			if (product.AvailableQuantity < cartItem.quantity)
				return CheckoutSessionErrors.InsufficientStock(cartItem.productId);

			var reserveResult = product.Reserve(cartItem.quantity);
			if (reserveResult.IsError)
				return reserveResult.Errors;

			checkoutItems.Add(CheckoutItem.Create(
				cartItem.productId,
				product.BasePrice,
				cartItem.quantity));
		}

		// 5. Create checkout session aggregate
		var checkoutSession = new CheckoutSession(customerId, cartDetails.MerchantId, addressId, checkoutItems);

		await _checkoutSessionRepository.AddAsync(checkoutSession, cancellationToken);
		await _checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}

	public async Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> CheckoutAsync(
		Guid checkoutSessionId,
		CancellationToken cancellationToken = default)
	{
		// 1. Get and validate the checkout session
		var checkoutSession = await _checkoutSessionRepository.GetByIdAsync(checkoutSessionId, cancellationToken);

		if (checkoutSession is null)
			return CheckoutSessionErrors.NotFound;

		// 2. Check if session is still active
		if (checkoutSession.Lifetime.IsExpired)
			return CheckoutSessionErrors.SessionExpired;

		// 3. Validate prices against Checkout's local Products
		var productIds = checkoutSession.Items.Select(i => i.ProductId).ToList();
		var checkoutProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
		var validationResult = checkoutSession.Validate(checkoutProducts);
		if (validationResult.IsError)
			return validationResult.Errors;

		// 4. Create payment session
		var paymentSession = await _sender.Send(
			new CreatePaymentSessionRequest(
				checkoutSession.CustomerId,
				checkoutSession.Id,
				checkoutSession.TotalPrice),
			cancellationToken);

		if (paymentSession.IsError)
			return paymentSession.Errors;

		// 5. Record the PaymentId on the session so payment success handlers can look it up
		checkoutSession.InitiatePayment(paymentSession.Value.PaymentId);
		await _checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		return (paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}
}
