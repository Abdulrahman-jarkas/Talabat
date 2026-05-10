using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Application.Shop.Queries.GetShop;

internal class GetShopQueryHandler(IShopsRepository shopsRepository)
    : IRequestHandler<ShopQuery, ErrorOr<ShopResponse>>
{
    public async Task<ErrorOr<ShopResponse>> Handle(ShopQuery request, CancellationToken cancellationToken)
    {
        var shop = await shopsRepository.GetByIdAsync(request.ShopId, cancellationToken);
        if (shop is null)
            return ShopErrors.NotFound(request.ShopId);

        return new ShopResponse(shop.Id, shop.Name, shop.Description);
    }
}
