using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Payments.Contracts;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
using Talabat.Users.Contracts;

namespace Talabat.Orders.Application.CheckoutSession.Commands.Checkout;

internal class CheckoutCommandHandler(
	ISender sender,
	ICheckoutSessionRepository checkoutSessionRepository) : IRequestHandler<CheckoutCommand, ErrorOr<CheckoutResult>>
{
	public async Task<ErrorOr<CheckoutResult>> Handle(CheckoutCommand command, CancellationToken cancellationToken)
	{
		// 1. Get the active checkout session for the customer
		var checkoutSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(command.CustomerId, cancellationToken);

		if (checkoutSession is null)
			return CheckoutSessionErrors.NotFound;

		if (checkoutSession.Lifetime.IsExpired)
			return CheckoutSessionErrors.SessionExpired;

		// 2. Validate the address belongs to the customer
		var customerDetails = await sender.Send(
			new CustomerDetailsQuery(checkoutSession.CustomerId),
			cancellationToken);

		if (customerDetails is null || !customerDetails.Addresses.Any(a => a.Id == command.AddressId))
			return CheckoutSessionErrors.AddressNotFound(command.AddressId);

		// 3. Validate prices against Products module
		var productIds = checkoutSession.Items.Select(i => i.ProductId).ToList();
		var productDetails = await sender.Send(new ProductsQuery(productIds), cancellationToken);

		if (productDetails is null || productDetails.Count == 0)
			return CheckoutSessionErrors.ProductsNotFound;

		// 4. Create payment session
		var paymentSession = await sender.Send(
			new CreatePaymentSessionRequest(
				checkoutSession.CustomerId,
				checkoutSession.Id,
				checkoutSession.TotalPrice),
			cancellationToken);

		if (paymentSession.IsError)
			return paymentSession.Errors;

		// 5. Checkout session with payment, address, and price validation
		var checkoutResult = checkoutSession.Checkout(
			paymentSession.Value.PaymentId,
			command.AddressId,
			productDetails.Select(p => (p.Id, p.BasePrice, p.Quantity)).ToList());
		if (checkoutResult.IsError)
			return checkoutResult.Errors;

		using var scope = ModuleTransactionScope.Create();

		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		scope.Complete();

		return new CheckoutResult(paymentSession.Value.PaymentId, paymentSession.Value.PaymentUrl);
	}
}

