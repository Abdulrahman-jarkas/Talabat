using ErrorOr;
using MediatR;
using Talabat.Orders.Contracts;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.CheckoutSessionAggregate;

namespace Talabat.Orders.Application.CheckoutSession.Queries.GetActiveCheckoutSession;

internal class GetActiveCheckoutSessionQueryHandler(ICheckoutSessionRepository checkoutSessionRepository)
    : IRequestHandler<GetActiveCheckoutSessionQuery, ErrorOr<CheckoutSessionResponse>>
{
    public async Task<ErrorOr<CheckoutSessionResponse>> Handle(
        GetActiveCheckoutSessionQuery request,
        CancellationToken cancellationToken)
    {
        var checkoutSession = await checkoutSessionRepository.GetActiveByCustomerIdAsync(
            request.CustomerId,
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
            checkoutSession.ShopId,
            checkoutSession.AddressId,
            items,
            checkoutSession.TotalPrice);
    }
}
