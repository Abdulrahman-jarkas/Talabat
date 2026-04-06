using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Products.Contracts;
using Talabat.Users.Contracts;
using CheckoutSessionAggregate = Talabat.Orders.Domain.CheckoutSessionAggregate;

namespace Talabat.Orders.Application.CheckoutSession.Commands.CreateCheckoutSession;

internal class CreateCheckoutSessionCommandHandler(
	ISender sender,
	ICheckoutSessionRepository checkoutSessionRepository) : IRequestHandler<CreateCheckoutSessionCommand, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(CreateCheckoutSessionCommand command, CancellationToken cancellationToken)
	{
		// 0. Check for any existing active session for this customer
		var existingSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(command.CustomerId, cancellationToken);
		if (existingSession is not null && existingSession.Lifetime.IsActive)
			return CheckoutSessionErrors.ActiveSessionAlreadyExists;

		// 1. Get customer details from Users module
		var customerDetails = await sender.Send(new CustomerDetailsQuery(command.CustomerId), cancellationToken);

		if (customerDetails?.Cart is null || customerDetails.Cart.Items.Count == 0)
			return CheckoutSessionErrors.CartEmpty;

		// 2. Get latest product details from Products module
		var productIds = customerDetails.Cart.Items.Select(ci => ci.ProductId).ToList();
		var productDetails = await sender.Send(new ProductsQuery(productIds), cancellationToken);

		if (productDetails is null || productDetails.Count == 0)
			return CheckoutSessionErrors.ProductsNotFound;

		// 3. Build checkout items and validate availability
		var checkoutItems = new List<CheckoutItem>();

		foreach (var cartItem in customerDetails.Cart.Items)
		{
			var product = productDetails.FirstOrDefault(p => p.Id == cartItem.ProductId);
			if (product is null)
				return CheckoutSessionErrors.ProductNotFound(cartItem.ProductId);

			if (product.Quantity < cartItem.Quantity)
				return CheckoutSessionErrors.InsufficientStock(cartItem.ProductId);

			checkoutItems.Add(CheckoutItem.Create(
				cartItem.ProductId,
				product.BasePrice,
				cartItem.Quantity));
		}

		// 4. Create checkout session aggregate
		var checkoutSession = new CheckoutSessionAggregate.CheckoutSession(command.CustomerId, customerDetails.Cart.MerchantId, checkoutItems);

		await checkoutSessionRepository.AddAsync(checkoutSession, cancellationToken);
		await checkoutSessionRepository.SaveChangesAsync(cancellationToken);

		return Result.Success;
	}
}
