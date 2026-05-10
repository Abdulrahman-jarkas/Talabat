using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Shop.Commands.DeleteShop;

internal record DeleteShopCommand(Guid ShopId) : IRequest<ErrorOr<Success>>;
