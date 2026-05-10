using ErrorOr;
using MediatR;
using Talabat.Accounts.Data;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Application.Role.Commands.RemoveRole;

internal class RemoveRoleCommandHandler(IRolesRepository rolesRepository)
    : IRequestHandler<RemoveRoleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RemoveRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await rolesRepository.GetByIdAsync(command.RoleId, command.TenantId, cancellationToken);

        if (role is null)
            return RoleErrors.NotFound;

        var versionCheck = ConcurrencyExtensions.EnsureVersion(role.Version, command.Version, () => RoleErrors.ConcurrencyConflict);
        if (versionCheck.IsError)
            return versionCheck.Errors;

        var result = role.Delete();
        if (result.IsError)
            return result.Errors;

        rolesRepository.Remove(role);

        return await rolesRepository.SaveWithConcurrencyAsync(
            () => Result.Success,
            () => RoleErrors.ConcurrencyConflict,
            cancellationToken);
    }
}
