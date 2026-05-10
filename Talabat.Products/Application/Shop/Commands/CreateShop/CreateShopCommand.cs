using ErrorOr;
using MediatR;

namespace Talabat.Products.Application.Shop.Commands.CreateShop;

internal record CreateShopCommand(string Name, string Description) : IRequest<ErrorOr<Guid>>;
