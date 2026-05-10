using ErrorOr;
using MediatR;
using Talabat.Accounts.Application.Account.Services;
using Talabat.Accounts.Data.Repositories;

namespace Talabat.Accounts.Application.Account.Commands.AddAccount;

internal class AddAccountCommandHandler(IAccountsRepository accountsRepository, IRolesRepository rolesRepository, IIdentityUserService identityUserService)
    : IRequestHandler<AddAccountCommand, ErrorOr<MutationResult>>
{
    public async Task<ErrorOr<MutationResult>> Handle(AddAccountCommand command, CancellationToken cancellationToken)
    {
        var user = await identityUserService.GetUserAsync(command.UserId.ToString(), cancellationToken);
        if (user is null)
            return Error.Validation("User.NotFound", "The specified user does not exist.");

        if (command.RoleIds.Count > 0)
        {
            var existingRoles = await rolesRepository.GetByIdsReadOnlyAsync(command.RoleIds, cancellationToken);
            var existingRoleIds = existingRoles.Select(r => r.Id).ToHashSet();
            var invalidRoleIds = command.RoleIds.Where(id => !existingRoleIds.Contains(id)).ToList();
            if (invalidRoleIds.Count > 0)
                return Error.Validation("Account.InvalidRoleIds", $"The following role IDs do not exist: {string.Join(", ", invalidRoleIds)}");
        }

        var account = Domain.AccountAggregate.Account.Create(
            command.UserId,
            user.FullName ?? $"{user.FirstName} {user.LastName}".Trim(),
            user.Email ?? string.Empty,
            command.TenantId,
            command.TenantType);

        if (command.RoleIds.Count > 0)
        {
            var result = account.UpdateAssignments(command.RoleIds, command.AssignedBy);
            if (result.IsError)
                return result.Errors;
        }

        await accountsRepository.AddAsync(account, cancellationToken);
        await accountsRepository.SaveChangesAsync(cancellationToken);

        return new MutationResult(account.Id, Convert.ToBase64String(account.Version));
    }
}
