using ErrorOr;
using MediatR;
using Talabat.Checkout.Contracts;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;

namespace Talabat.Orders.Integration;

internal class GetCheckoutSessionByPaymentIdQueryHandler(ICheckoutSessionRepository checkoutSessionRepository)
	: IRequestHandler<GetCheckoutSessionByPaymentIdQuery, ErrorOr<CheckoutSessionResponse>>
{
	public async Task<ErrorOr<CheckoutSessionResponse>> Handle(
		GetCheckoutSessionByPaymentIdQuery request,
		CancellationToken cancellationToken)
	{
		var checkoutSession = await checkoutSessionRepository.GetByPaymentIdAsync(
			request.PaymentId,
			cancellationToken);

		if (checkoutSession is null)
			return CheckoutSessionErrors.NotFound;

		var items = checkoutSession.Items
			.Select(item => new CheckoutSessionItemDto(
				item.ProductId,
				item.Price,
				item.Quantity))
			.ToList();

		return new CheckoutSessionResponse(
			checkoutSession.Id,
			checkoutSession.CustomerId,
			checkoutSession.MerchantId,
			checkoutSession.AddressId,
			items,
			checkoutSession.TotalPrice);
	}
}
