using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Shop.Commands.UpdateShop;

internal record UpdateShopCommand(Guid ShopId, string Name, string Description) : IRequest<ErrorOr<Success>>;
