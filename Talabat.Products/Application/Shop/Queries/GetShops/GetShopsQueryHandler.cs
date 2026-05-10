using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Application.Shop.Queries.GetShops;

internal class GetShopsQueryHandler(IShopsRepository shopsRepository)
    : IRequestHandler<ShopsQuery, IReadOnlyList<ShopResponse>>
{
    public async Task<IReadOnlyList<ShopResponse>> Handle(ShopsQuery request, CancellationToken cancellationToken)
    {
        var shops = await shopsRepository.GetAllAsync(cancellationToken);

        return shops
            .Select(s => new ShopResponse(s.Id, s.Name, s.Description))
            .ToList();
    }
}
