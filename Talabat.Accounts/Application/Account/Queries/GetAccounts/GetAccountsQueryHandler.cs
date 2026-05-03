using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;

namespace Talabat.Accounts.Application.Account.Queries.GetAccounts;

internal class GetAccountsQueryHandler(IAccountsRepository accountsRepository, IRolesRepository rolesRepository)
    : IRequestHandler<GetAccountsQuery, ErrorOr<List<AccountDto>>>
{
    public async Task<ErrorOr<List<AccountDto>>> Handle(GetAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await accountsRepository.GetAccountsByTenantAsync(query.TenantId, cancellationToken);
        var roles = await rolesRepository.GetByTenantAsync(query.TenantId, cancellationToken);
        var roleNames = roles.ToDictionary(r => r.Id, r => r.Name);

        var dtos = accounts.Select(a => new AccountDto(
            a.Id,
            a.User.UserId,
            a.User.Name,
            a.User.Email,
            a.Tenant.TenantId,
            a.Tenant.TenantType.ToString(),
            a.AccountRoles.Select(ar => new AccountRoleDto(
                ar.RoleId,
                roleNames.GetValueOrDefault(ar.RoleId),
                ar.AssignedBy,
                ar.AssignedAt)).ToList()
        )).ToList();

        return dtos;
    }
}
