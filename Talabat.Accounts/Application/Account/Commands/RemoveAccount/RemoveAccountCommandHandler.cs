using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Account.Commands.RemoveAccount;

internal class RemoveAccountCommandHandler(IAccountsRepository accountsRepository)
    : IRequestHandler<RemoveAccountCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RemoveAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await accountsRepository.GetByIdAsync(command.AccountId, cancellationToken);

        if (account is null || account.Tenant.TenantId != command.TenantId)
            return AccountErrors.NotFound;

        var result = account.Remove();
        if (result.IsError)
            return result.Errors;

        using var scope = ModuleTransactionScope.Create();

        await accountsRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return Result.Success;
    }
}
