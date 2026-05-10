using ErrorOr;
using MediatR;
using Talabat.Accounts.Data;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.AccountAggregate;

namespace Talabat.Accounts.Application.Account.Commands.RemoveAccount;

internal class RemoveAccountCommandHandler(IAccountsRepository accountsRepository)
    : IRequestHandler<RemoveAccountCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RemoveAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await accountsRepository.GetByIdAsync(command.AccountId, command.TenantId, cancellationToken);

        if (account is null)
            return AccountErrors.NotFound;

        var versionCheck = ConcurrencyExtensions.EnsureVersion(account.Version, command.Version, () => AccountErrors.ConcurrencyConflict);
        if (versionCheck.IsError)
            return versionCheck.Errors;

        var result = account.Remove();
        if (result.IsError)
            return result.Errors;

        return await accountsRepository.SaveWithConcurrencyAsync(
            () => Result.Success,
            () => AccountErrors.ConcurrencyConflict,
            cancellationToken);
    }
}
