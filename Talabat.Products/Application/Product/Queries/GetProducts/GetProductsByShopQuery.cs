using MediatR;
using Talabat.Products.Contracts;

namespace Talabat.Products.Application.Product.Queries.GetProducts;

internal record GetProductsByShopQuery(Guid? ShopId) : IRequest<List<ProductResponse>>;
