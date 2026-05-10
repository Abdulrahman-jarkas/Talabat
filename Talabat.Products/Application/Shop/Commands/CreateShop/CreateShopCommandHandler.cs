using ErrorOr;
using MediatR;
using Talabat.Products.Data.Repositories;
using Talabat.SharedKernal;

namespace Talabat.Products.Application.Shop.Commands.CreateShop;

internal class CreateShopCommandHandler(IShopsRepository shopsRepository)
    : IRequestHandler<CreateShopCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(CreateShopCommand command, CancellationToken cancellationToken)
    {
        using var scope = ModuleTransactionScope.Create();

        var shop = new Domain.Shop(command.Name, command.Description);

        await shopsRepository.AddAsync(shop, cancellationToken);
        await shopsRepository.SaveChangesAsync();

        scope.Complete();

        return shop.Id;
    }
}
