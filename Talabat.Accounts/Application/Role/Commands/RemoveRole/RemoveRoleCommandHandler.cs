using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Role.Commands.RemoveRole;

internal class RemoveRoleCommandHandler(IRolesRepository rolesRepository, IAccountsRepository accountsRepository)
    : IRequestHandler<RemoveRoleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RemoveRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await rolesRepository.GetByIdAsync(command.RoleId, cancellationToken);

        if (role is null || role.Tenant.TenantId != command.TenantId)
            return RoleErrors.NotFound;

        var hasAccounts = await accountsRepository.HasAccountsWithRoleAsync(command.RoleId, cancellationToken);
        if (hasAccounts)
            return RoleErrors.InUse;

        var result = role.Delete();
        if (result.IsError)
            return result.Errors;

        using var scope = ModuleTransactionScope.Create();

        await rolesRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return Result.Success;
    }
}
