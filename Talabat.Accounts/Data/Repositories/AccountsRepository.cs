using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Domain.AccountAggregate;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Data.Repositories;

internal class AccountsRepository(AccountsDbContext dbContext) : IAccountsRepository
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(Guid accountId, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Accounts
            .Include(a => a.Assignments)
            .Where(a => a.Id == accountId);

        if (tenantId.HasValue)
            query = query.Where(a => a.Tenant.TenantId == tenantId.Value);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Account>> GetAccountsByTenantReadOnlyAsync(string? tenantType, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Accounts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(tenantType) && Enum.TryParse<TenantType>(tenantType, ignoreCase: true, out var parsed))
            query = query.Where(a => a.Tenant.TenantType == parsed);

        if (tenantId.HasValue)
            query = query.Where(a => a.Tenant.TenantId == tenantId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Account>> GetAccountsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Accounts
            .Include(a => a.Assignments)
            .Where(a => a.Assignments.Any(ar => ar.RoleId == roleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Account>> GetUserAccountsReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.User.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ErrorOr<T>> SaveWithConcurrencyAsync<T>(Func<T> successFactory, Func<Error> concurrencyError, CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveWithConcurrencyAsync(successFactory, concurrencyError, cancellationToken);
    }
}
