using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Account.Commands.AddAccount;

internal class AddAccountCommandHandler(IAccountsRepository accountsRepository)
    : IRequestHandler<AddAccountCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(AddAccountCommand command, CancellationToken cancellationToken)
    {
        var account = Domain.AccountAggregate.Account.Create(
            command.UserId,
            command.Name,
            command.Email,
            command.TenantId,
            command.TenantType);

        if (command.RoleIds.Count > 0)
        {
            var result = account.SetRoles(command.RoleIds, command.AssignedBy);
            if (result.IsError)
                return result.Errors;
        }

        using var scope = ModuleTransactionScope.Create();

        await accountsRepository.AddAsync(account, cancellationToken);
        await accountsRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return account.Id;
    }
}
