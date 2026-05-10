using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Data;
using Talabat.Accounts.Data.Repositories;
using Talabat.Accounts.Domain.AccountAggregate;

namespace Talabat.Accounts.Application.Account.Commands.UpdateAccount;

internal class UpdateAccountCommandHandler(IAccountsRepository accountsRepository, IRolesRepository rolesRepository, AccountsDbContext dbContext)
    : IRequestHandler<UpdateAccountCommand, ErrorOr<MutationResult>>
{
    public async Task<ErrorOr<MutationResult>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await accountsRepository.GetByIdAsync(command.AccountId, command.TenantId, cancellationToken);

        if (account is null)
            return AccountErrors.NotFound;

        var versionCheck = ConcurrencyExtensions.EnsureVersion(account.Version, command.Version, () => AccountErrors.ConcurrencyConflict);
        if (versionCheck.IsError)
            return versionCheck.Errors;

        if (command.RoleIds.Count > 0)
        {
            var existingRoles = await rolesRepository.GetByIdsReadOnlyAsync(command.RoleIds, cancellationToken);
            var existingRoleIds = existingRoles.Select(r => r.Id).ToHashSet();
            var invalidRoleIds = command.RoleIds.Where(id => !existingRoleIds.Contains(id)).ToList();
            if (invalidRoleIds.Count > 0)
                return Error.Validation("Account.InvalidRoleIds", $"The following role IDs do not exist: {string.Join(", ", invalidRoleIds)}");
        }

        var result = account.UpdateAssignments(command.RoleIds, command.AssignedBy);
        if (result.IsError)
            return result.Errors;

        var (added, removed) = result.Value;

        foreach (var assignment in removed)
            dbContext.Entry(assignment).State = EntityState.Deleted;

        foreach (var assignment in added)
            dbContext.Entry(assignment).State = EntityState.Added;

        return await accountsRepository.SaveWithConcurrencyAsync(
            () => new MutationResult(account.Id, Convert.ToBase64String(account.Version)),
            () => AccountErrors.ConcurrencyConflict,
            cancellationToken);
    }
}
