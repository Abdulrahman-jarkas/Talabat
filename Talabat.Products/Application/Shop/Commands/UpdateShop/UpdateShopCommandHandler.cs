using ErrorOr;
using MediatR;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Application.Shop.Commands.UpdateShop;

internal class UpdateShopCommandHandler(IShopsRepository shopsRepository)
    : IRequestHandler<UpdateShopCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateShopCommand command, CancellationToken cancellationToken)
    {
        var shop = await shopsRepository.GetByIdAsync(command.ShopId, cancellationToken);
        if (shop is null)
            return ShopErrors.NotFound(command.ShopId);

        var result = shop.Update(command.Name, command.Description);
        if (result.IsError)
            return result.Errors;

        await shopsRepository.SaveChangesAsync();

        return Result.Success;
    }
}
