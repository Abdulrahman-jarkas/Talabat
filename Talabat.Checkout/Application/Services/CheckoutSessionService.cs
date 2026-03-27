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
	private readonly IProductService _productService;

	public CheckoutSessionService(
		ISender sender,
		ICheckoutSessionRepository checkoutSessionRepository,
		IProductService productService)
	{
		_sender = sender;
		_checkoutSessionRepository = checkoutSessionRepository;
		_productService = productService;
	}

	public async Task<ErrorOr<Success>> CreateAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
	{
		// 0. Check and expire any existing active session for this customer
		var existingSession = await _checkoutSessionRepository.GetActiveByCustomerIdAsync(customerId, cancellationToken);
		if (existingSession is not null)
		{
			if (existingSession.TryExpireIfLifetimeExceeded())
			{
				// Release reserved stock for the expired session
				var stockItems = existingSession.Items
					.Select(i => (i.ProductId, i.Quantity)).ToList();
				await _productService.ReleaseStockAsync(stockItems, cancellationToken);
				await _checkoutSessionRepository.SaveChangesAsync(cancellationToken);
			}
			else
			{
				return CheckoutSessionErrors.ActiveSessionAlreadyExists;
			}
		}

		// 1. Get cart details from Users context
		var cartDetails = await _sender.Send(new CartDetailsQuery(customerId), cancellationToken);

		if (cartDetails is null || cartDetails.CartItems.Count == 0)
			return CheckoutSessionErrors.CartEmpty;

		// 2. Get product details to populate prices and validate stock
		var productIds = cartDetails.CartItems.Select(ci => ci.productId).ToList();
		var products = await _productService.GetProductsForValidationAsync(productIds, cancellationToken);

		if (products is null || products.Count == 0)
			return CheckoutSessionErrors.ProductsNotFound;

		// 3. Build checkout items from cart + product data, validate availability
		var checkoutItems = new List<CheckoutItem>();
		foreach (var cartItem in cartDetails.CartItems)
		{
			var product = products.FirstOrDefault(p => p.ProductId == cartItem.productId);
			if (product is null)
				return CheckoutSessionErrors.ProductNotFound(cartItem.productId);

			if (product.EffectiveStock < cartItem.quantity)
				return CheckoutSessionErrors.InsufficientStock(cartItem.productId);

			checkoutItems.Add(CheckoutItem.Create(
				cartItem.productId,
				product.BasePrice,
				cartItem.quantity));
		}

		// 4. Reserve stock in the Products context
		var reserveItems = checkoutItems.Select(i => (i.ProductId, i.Quantity)).ToList();
		var reserveResult = await _productService.ReserveStockAsync(reserveItems, cancellationToken);
		if (reserveResult.IsError)
			return reserveResult.Errors;

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

		// 2. Check for auto-expiration
		if (checkoutSession.TryExpireIfLifetimeExceeded())
		{
			var stockItems = checkoutSession.Items
				.Select(i => (i.ProductId, i.Quantity)).ToList();
			await _productService.ReleaseStockAsync(stockItems, cancellationToken);
			await _checkoutSessionRepository.SaveChangesAsync(cancellationToken);
			return CheckoutSessionErrors.SessionExpired;
		}

		var validationResult = await checkoutSession.Validate(_productService, cancellationToken);
		if (validationResult.IsError)
			return validationResult.Errors;

		// 3. Create payment session
		var paymentSession = await _sender.Send(
			new CreatePaymentSessionRequest(
				checkoutSession.CustomerId,
				checkoutSession.Id,
				checkoutSession.TotalPrice),
			cancellationToken);

		if (paymentSession.IsError)
			return paymentSession.Errors;

		await _checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		return (paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}
}
