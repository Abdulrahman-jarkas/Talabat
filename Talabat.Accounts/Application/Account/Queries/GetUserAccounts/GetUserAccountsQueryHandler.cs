using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;

namespace Talabat.Accounts.Application.Account.Queries.GetUserAccounts;

internal class GetUserAccountsQueryHandler(IAccountsRepository accountsRepository, IRolesRepository rolesRepository)
    : IRequestHandler<GetUserAccountsQuery, ErrorOr<List<UserAccountDto>>>
{
    public async Task<ErrorOr<List<UserAccountDto>>> Handle(GetUserAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await accountsRepository.GetUserAccountsReadOnlyAsync(query.UserId, cancellationToken);

        // Collect all role IDs across all accounts and load them in a single query
        var allRoleIds = accounts
            .SelectMany(a => a.Assignments.Select(ar => ar.RoleId))
            .Distinct();

        var rolesList = await rolesRepository.GetByIdsReadOnlyAsync(allRoleIds, cancellationToken);
        var roles = rolesList.ToDictionary(r => r.Id);

        var dtos = accounts.Select(a =>
        {
            var accountRoles = a.Assignments
                .Where(ar => roles.ContainsKey(ar.RoleId))
                .Select(ar =>
                {
                    var role = roles[ar.RoleId];
                    return new UserAccountRoleDto(
                        ar.RoleId,
                        role.Name,
                        role.Permissions);
                })
                .ToList();

            return new UserAccountDto(
                a.Id,
                a.User.Name,
                a.Tenant.TenantType.ToString(),
                a.Tenant.TenantId,
                Convert.ToBase64String(a.Version),
                accountRoles);
        }).ToList();

        return dtos;
    }
}
