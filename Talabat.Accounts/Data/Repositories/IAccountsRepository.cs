using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Data.Repositories;

internal interface IAccountsRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<List<Account>> GetAccountsByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<List<Account>> GetAccountsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Account>> GetUserAccountsAsync(Guid userId, string? tenantType, Guid? tenantId, CancellationToken cancellationToken = default);
    Task<bool> HasAccountsWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

internal interface IRolesRepository
{
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? tenantId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
