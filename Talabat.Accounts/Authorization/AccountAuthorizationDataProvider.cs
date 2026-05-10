using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Data;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Authorization;

internal class AccountAuthorizationDataProvider : IAccountAuthorizationDataProvider
{
    private readonly AccountsDbContext _dbContext;

    public AccountAuthorizationDataProvider(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountAuthorizationData?> GetAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts
            .Include(a => a.Assignments)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account is null || account.IsDeleted)
            return null;

        // Load roles for this account's role IDs
        var roleIds = account.Assignments.Select(ar => ar.RoleId).ToList();

        var roles = await _dbContext.Roles
            .AsNoTracking()
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var roleNames = roles.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in roles)
        {
            foreach (var p in role.Permissions)
                permissions.Add(p);
        }

        return new AccountAuthorizationData
        {
            AccountId = account.Id,
            UserId = account.User.UserId,
            TenantId = account.Tenant.TenantId,
            TenantType = account.Tenant.TenantType.ToString(),
            AccountName = account.User.Name,
            IsDeleted = account.IsDeleted,
            Version = account.Version,
            Roles = roleNames,
            Permissions = permissions
        };
    }
}
