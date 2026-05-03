using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Domain.AccountAggregate;

namespace Talabat.Accounts.Data.Repositories;

internal class AccountsRepository(AccountsDbContext dbContext) : IAccountsRepository
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
    }

    public async Task<List<Account>> GetAccountsByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Accounts
            .Where(a => a.Tenant.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Account>> GetAccountsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var allAccounts = await dbContext.Accounts.ToListAsync(cancellationToken);
        return allAccounts.Where(a => a.AccountRoles.Any(ar => ar.RoleId == roleId)).ToList();
    }

    public async Task<List<Account>> GetUserAccountsAsync(Guid userId, string? tenantType, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Accounts.Where(a => a.User.UserId == userId);

        if (!string.IsNullOrEmpty(tenantType))
            query = query.Where(a => a.Tenant.TenantType.ToString() == tenantType);

        if (tenantId.HasValue)
            query = query.Where(a => a.Tenant.TenantId == tenantId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAccountsWithRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleIdString = roleId.ToString();
        return await dbContext.Accounts
            .AnyAsync(a => EF.Property<string>(a, "AccountRoles").Contains(roleIdString), cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
