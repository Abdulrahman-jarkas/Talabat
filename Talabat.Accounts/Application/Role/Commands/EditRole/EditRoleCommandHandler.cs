using ErrorOr;
using MediatR;
using Talabat.Accounts.Data;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Application.Role.Commands.EditRole;

internal class EditRoleCommandHandler(IRolesRepository rolesRepository)
    : IRequestHandler<EditRoleCommand, ErrorOr<MutationResult>>
{
    public async Task<ErrorOr<MutationResult>> Handle(EditRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await rolesRepository.GetByIdAsync(command.RoleId, command.TenantId, cancellationToken);

        if (role is null)
            return RoleErrors.NotFound;

        var versionCheck = ConcurrencyExtensions.EnsureVersion(role.Version, command.Version, () => RoleErrors.ConcurrencyConflict);
        if (versionCheck.IsError)
            return versionCheck.Errors;

        var updateResult = role.Update(command.Name, command.ModifiedBy);
        if (updateResult.IsError)
            return updateResult.Errors;

        if (!role.Permissions.SequenceEqual(command.Permissions))
        {
            var permResult = role.ChangePermissions(command.Permissions, command.ModifiedBy);
            if (permResult.IsError)
                return permResult.Errors;
        }

        return await rolesRepository.SaveWithConcurrencyAsync(
            () => new MutationResult(role.Id, Convert.ToBase64String(role.Version)),
            () => RoleErrors.ConcurrencyConflict,
            cancellationToken);
    }
}
