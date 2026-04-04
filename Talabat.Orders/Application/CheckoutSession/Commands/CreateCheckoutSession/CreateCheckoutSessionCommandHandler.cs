using ErrorOr;
using MediatR;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
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

		// 1. Get cart details from Users module
		var cartDetails = await sender.Send(new CartDetailsQuery(command.CustomerId), cancellationToken);

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
		var checkoutSession = new CheckoutSessionAggregate.CheckoutSession(command.CustomerId, cartDetails.MerchantId, command.AddressId, checkoutItems);

		// 5. Reserve products + save session in a single transaction
		foreach (var item in checkoutItems)
		{
			reservationItems.Add(new AddReservationItem(
				item.ProductId,
				checkoutSession.Id,
				command.CustomerId,
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
}
