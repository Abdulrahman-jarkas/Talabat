using Microsoft.EntityFrameworkCore;
using Talabat.Accounts.Domain.RoleAggregate;

namespace Talabat.Accounts.Data.Repositories;

internal class RolesRepository(AccountsDbContext dbContext) : IRolesRepository
{
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await dbContext.Roles.AddAsync(role, cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<List<Role>> GetByIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        var ids = roleIds.ToList();
        return await dbContext.Roles
            .Where(r => ids.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Role>> GetByTenantAsync(Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .Where(r => r.Tenant.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .AnyAsync(r => r.Name == name && r.Tenant.TenantId == tenantId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
