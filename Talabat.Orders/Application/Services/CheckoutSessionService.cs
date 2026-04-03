using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
using Talabat.Users.Contracts;
using CheckoutSessionAggregate = Talabat.Orders.Domain.CheckoutSessionAggregate;

namespace Talabat.Orders.Application.Services;

internal class CheckoutSessionService(
	ISender sender,
	ICheckoutSessionRepository checkoutSessionRepository) : ICheckoutSessionService
{
	public async Task<ErrorOr<Success>> CreateAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
	{
		// 0. Check for any existing active session for this customer
		var existingSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(customerId, cancellationToken);
		if (existingSession is not null && existingSession.Lifetime.IsActive)
			return CheckoutSessionErrors.ActiveSessionAlreadyExists;

		// 1. Get cart details from Users module
		var cartDetails = await sender.Send(new CartDetailsQuery(customerId), cancellationToken);

		if (cartDetails is null || cartDetails.CartItems.Count == 0)
			return CheckoutSessionErrors.CartEmpty;

		// 2. Get latest product details from Products module
		var productIds = cartDetails.CartItems.Select(ci => ci.productId).ToList();
		var productDetails = await sender.Send(new ProductsQuery(productIds), cancellationToken);

		if (productDetails is null || productDetails.Count == 0)
			return CheckoutSessionErrors.ProductsNotFound;

		// 3. Build checkout items and validate availability
		var checkoutItems = new List<CheckoutItem>();
		var reservationItems = new List<AddReservationItem>();

		foreach (var cartItem in cartDetails.CartItems)
		{
			var product = productDetails.FirstOrDefault(p => p.Id == cartItem.productId);
			if (product is null)
				return CheckoutSessionErrors.ProductNotFound(cartItem.productId);

			if (product.Quantity < cartItem.quantity)
				return CheckoutSessionErrors.InsufficientStock(cartItem.productId);

			checkoutItems.Add(CheckoutItem.Create(
				cartItem.productId,
				product.BasePrice,
				cartItem.quantity));
		}

		// 4. Create checkout session aggregate
		var checkoutSession = new CheckoutSessionAggregate.CheckoutSession(customerId, cartDetails.MerchantId, addressId, checkoutItems);

		// 5. Reserve products + save session in a single transaction
		foreach (var item in checkoutItems)
		{
			reservationItems.Add(new AddReservationItem(
				item.ProductId,
				checkoutSession.Id,
				customerId,
				item.Quantity));
		}

		using var scope = ModuleTransactionScope.Create();

		var reservationResult = await sender.Send(new AddReservationRequest(reservationItems), cancellationToken);
		if (reservationResult.IsError)
			return reservationResult.Errors;

		await checkoutSessionRepository.AddAsync(checkoutSession, cancellationToken);
		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		return Result.Success;
	}

	public async Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> CheckoutAsync(
		Guid checkoutSessionId,
		CancellationToken cancellationToken = default)
	{
		// 1. Get and validate the checkout session
		var checkoutSession = await checkoutSessionRepository.GetByIdAsync(checkoutSessionId, cancellationToken);

		if (checkoutSession is null)
			return CheckoutSessionErrors.NotFound;

		if (checkoutSession.Lifetime.IsExpired)
			return CheckoutSessionErrors.SessionExpired;

		// 2. Validate prices against Products module
		var productIds = checkoutSession.Items.Select(i => i.ProductId).ToList();
		var productDetails = await sender.Send(new ProductsQuery(productIds), cancellationToken);

		if (productDetails is null || productDetails.Count == 0)
			return CheckoutSessionErrors.ProductsNotFound;

		var currentPrices = productDetails.ToDictionary(p => p.Id, p => p.BasePrice);
		var validationResult = checkoutSession.ValidatePrices(currentPrices);
		if (validationResult.IsError)
			return validationResult.Errors;

		// 3. Create payment session
		var paymentSession = await sender.Send(
			new CreatePaymentSessionRequest(
				checkoutSession.CustomerId,
				checkoutSession.Id,
				checkoutSession.TotalPrice),
			cancellationToken);

		if (paymentSession.IsError)
			return paymentSession.Errors;

		// 4. Record payment on checkout session
		var initiateResult = checkoutSession.InitiatePayment(paymentSession.Value.PaymentId);
		if (initiateResult.IsError)
			return initiateResult.Errors;

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		return (paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}
}
