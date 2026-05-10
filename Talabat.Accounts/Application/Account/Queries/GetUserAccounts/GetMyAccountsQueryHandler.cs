using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;

namespace Talabat.Accounts.Application.Account.Queries.GetUserAccounts;

internal class GetMyAccountsQueryHandler(IAccountsRepository accountsRepository, IRolesRepository rolesRepository)
    : IRequestHandler<GetMyAccountsQuery, ErrorOr<List<MyAccountDto>>>
{
    public async Task<ErrorOr<List<MyAccountDto>>> Handle(GetMyAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await accountsRepository.GetUserAccountsReadOnlyAsync(
            query.UserId, cancellationToken);

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
                    return new MyAccountRoleDto(
                        ar.RoleId,
                        role.Name,
                        role.Permissions);
                })
                .ToList();

            return new MyAccountDto(
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
