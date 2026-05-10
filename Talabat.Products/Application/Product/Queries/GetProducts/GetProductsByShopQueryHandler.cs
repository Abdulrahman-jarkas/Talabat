using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Application.Product.Queries.GetProducts;

internal class GetProductsByShopQueryHandler(IProductsRepository productsRepository)
    : IRequestHandler<GetProductsByShopQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetProductsByShopQuery request, CancellationToken cancellationToken)
    {
        var products = request.ShopId.HasValue
            ? await productsRepository.GetProductsByShopIdAsync(request.ShopId.Value, cancellationToken)
            : await productsRepository.GetAllProductsAsync(cancellationToken);

        return products
            .Select(p => new ProductResponse(
                p.Id,
                p.Title,
                p.ShopId,
                p.BasePrice,
                p.Stock.EffectiveQuantity))
            .ToList();
    }
}
