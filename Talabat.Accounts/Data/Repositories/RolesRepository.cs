using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Domain.RoleAggregate;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Data.Repositories;

internal class RolesRepository(AccountsDbContext dbContext) : IRolesRepository
{
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await dbContext.Roles.AddAsync(role, cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid roleId, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Roles.Where(r => r.Id == roleId);

        if (tenantId.HasValue)
            query = query.Where(r => r.Tenant.TenantId == tenantId.Value);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Role>> GetByTenantReadOnlyAsync(string? tenantType, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(tenantType) && Enum.TryParse<TenantType>(tenantType, ignoreCase: true, out var parsed))
            query = query.Where(r => r.Tenant.TenantType == parsed);

        if (tenantId.HasValue)
            query = query.Where(r => r.Tenant.TenantId == tenantId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Role>> GetByIdsReadOnlyAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var ids = roleIds.ToList();
        return await dbContext.Roles
            .AsNoTracking()
            .Where(r => ids.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .AnyAsync(r => r.Name == name && r.Tenant.TenantId == tenantId, cancellationToken);
    }

    public void Remove(Role role)
    {
        dbContext.Roles.Remove(role);
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
