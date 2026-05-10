using ErrorOr;
using MediatR;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Application.Shop.Commands.DeleteShop;

internal class DeleteShopCommandHandler(IShopsRepository shopsRepository)
    : IRequestHandler<DeleteShopCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteShopCommand command, CancellationToken cancellationToken)
    {
        var shop = await shopsRepository.GetByIdAsync(command.ShopId, cancellationToken);
        if (shop is null)
            return ShopErrors.NotFound(command.ShopId);

        var result = shop.SoftDelete();
        if (result.IsError)
            return result.Errors;

        await shopsRepository.SaveChangesAsync();

        return Result.Success;
    }
}
