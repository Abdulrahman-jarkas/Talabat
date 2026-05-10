using ErrorOr;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Data.Repositories;

internal interface IAccountsRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(Guid accountId, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<List<Account>> GetAccountsByTenantReadOnlyAsync(string? tenantType, Guid? tenantId, CancellationToken cancellationToken = default);
    Task<List<Account>> GetAccountsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Account>> GetUserAccountsReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<T>> SaveWithConcurrencyAsync<T>(Func<T> successFactory, Func<Error> concurrencyError, CancellationToken cancellationToken = default);
}

internal interface IRolesRepository
{
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(Guid roleId, Guid? tenantId = null, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByTenantReadOnlyAsync(string? tenantType, Guid? tenantId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByIdsReadOnlyAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? tenantId, CancellationToken cancellationToken = default);
    void Remove(Role role);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<T>> SaveWithConcurrencyAsync<T>(Func<T> successFactory, Func<Error> concurrencyError, CancellationToken cancellationToken = default);
}
