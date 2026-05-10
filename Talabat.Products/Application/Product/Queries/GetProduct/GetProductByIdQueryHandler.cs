using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Application.Product.Queries.GetProduct;

internal class GetProductByIdQueryHandler(IProductsRepository productsRepository)
    : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductResponse>>
{
    public async Task<ErrorOr<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetProductByIdAsync(request.ProductId, request.TenantId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound(request.ProductId);

        return new ProductResponse(
            product.Id,
            product.Title,
            product.ShopId,
            product.BasePrice,
            product.Stock.EffectiveQuantity);
    }
}
