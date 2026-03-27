using ErrorOr;
using MediatR;
using Talabat.Checkout.Contracts;
using Talabat.Checkout.Data.Repositories;
using Talabat.Checkout.Domain.CheckoutSessionAggregate;

namespace Talabat.Checkout.Integration;

internal class GetCheckoutSessionQueryHandler(ICheckoutSessionRepository checkoutSessionRepository)
	: IRequestHandler<GetCheckoutSessionQuery, ErrorOr<CheckoutSessionResponse>>
{
	public async Task<ErrorOr<CheckoutSessionResponse>> Handle(
		GetCheckoutSessionQuery request,
		CancellationToken cancellationToken)
	{
		var checkoutSession = await checkoutSessionRepository.GetByIdAsync(
			request.CheckoutSessionId,
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
