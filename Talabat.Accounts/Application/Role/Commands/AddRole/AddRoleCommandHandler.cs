using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Application.Role.Commands.AddRole;

internal class AddRoleCommandHandler(IRolesRepository rolesRepository)
    : IRequestHandler<AddRoleCommand, ErrorOr<MutationResult>>
{
    public async Task<ErrorOr<MutationResult>> Handle(AddRoleCommand command, CancellationToken cancellationToken)
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

        await rolesRepository.AddAsync(role, cancellationToken);

        try
        {
            await rolesRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return RoleErrors.DuplicateName;
        }

        return new MutationResult(role.Id, Convert.ToBase64String(role.Version));
    }
}
