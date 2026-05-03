using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Role.Commands.AddRole;

internal class AddRoleCommandHandler(IRolesRepository rolesRepository)
    : IRequestHandler<AddRoleCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(AddRoleCommand command, CancellationToken cancellationToken)
    {
        var exists = await rolesRepository.ExistsByNameAsync(command.Name, command.TenantId, cancellationToken);
        if (exists)
            return RoleErrors.DuplicateName;

        var roleResult = Domain.RoleAggregate.Role.Create(
            command.Name,
            command.Permissions,
            command.TenantId,
            command.TenantType,
            command.CreatedBy);

        if (roleResult.IsError)
            return roleResult.Errors;

        var role = roleResult.Value;

        using var scope = ModuleTransactionScope.Create();

        await rolesRepository.AddAsync(role, cancellationToken);
        await rolesRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return role.Id;
    }
}
