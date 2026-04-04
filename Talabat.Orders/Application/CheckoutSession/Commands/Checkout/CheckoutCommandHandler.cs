using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;

namespace Talabat.Orders.Application.CheckoutSession.Commands.Checkout;

internal class CheckoutCommandHandler(
	ISender sender,
	ICheckoutSessionRepository checkoutSessionRepository) : IRequestHandler<CheckoutCommand, ErrorOr<CheckoutResult>>
{
	public async Task<ErrorOr<CheckoutResult>> Handle(CheckoutCommand command, CancellationToken cancellationToken)
	{
		// 1. Get and validate the checkout session
		var checkoutSession = await checkoutSessionRepository.GetByIdAsync(command.CheckoutSessionId, cancellationToken);

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

		return new CheckoutResult(paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}
}
