using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Account.Commands.UpdateAccount;

internal class UpdateAccountCommandHandler(IAccountsRepository accountsRepository)
    : IRequestHandler<UpdateAccountCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await accountsRepository.GetByIdAsync(command.AccountId, cancellationToken);

        if (account is null || account.Tenant.TenantId != command.TenantId)
            return AccountErrors.NotFound;

        var nameResult = account.UpdateName(command.Name);
        if (nameResult.IsError)
            return nameResult.Errors;

        var rolesResult = account.SetRoles(command.RoleIds, command.AssignedBy);
        if (rolesResult.IsError)
            return rolesResult.Errors;

        using var scope = ModuleTransactionScope.Create();

        await accountsRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return Result.Success;
    }
}
