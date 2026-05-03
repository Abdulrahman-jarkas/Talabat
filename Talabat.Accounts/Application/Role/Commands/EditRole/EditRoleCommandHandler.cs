using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Application.Role.Commands.EditRole;

internal class EditRoleCommandHandler(IRolesRepository rolesRepository)
    : IRequestHandler<EditRoleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(EditRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await rolesRepository.GetByIdAsync(command.RoleId, cancellationToken);

        if (role is null || role.Tenant.TenantId != command.TenantId)
            return RoleErrors.NotFound;

        var updateResult = role.Update(command.Name, command.ModifiedBy);
        if (updateResult.IsError)
            return updateResult.Errors;

        if (!role.Permissions.SequenceEqual(command.Permissions))
        {
            var permResult = role.ChangePermissions(command.Permissions, command.ModifiedBy);
            if (permResult.IsError)
                return permResult.Errors;
        }

        using var scope = ModuleTransactionScope.Create();

        await rolesRepository.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return Result.Success;
    }
}
