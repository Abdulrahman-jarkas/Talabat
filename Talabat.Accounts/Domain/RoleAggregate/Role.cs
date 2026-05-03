using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.Accounts.Domain.RoleAggregate.Events;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.RoleAggregate;

internal class Role : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public List<string> Permissions { get; private set; } = new();
    public Tenant Tenant { get; private set; } = null!;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    internal static ErrorOr<Role> Create(
        string name,
        List<string> permissions,
        Guid? tenantId,
        TenantType tenantType,
        Guid createdBy)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrEmpty(permissions);

        if (tenantType == TenantType.Shop)
            Guard.Against.Null(tenantId);

        if (tenantType != TenantType.System && tenantType != TenantType.Shop)
            return RoleErrors.InvalidTenantType;

        var role = new Role(Guid.NewGuid())
        {
            Name = name,
            Permissions = permissions,
            Tenant = new Tenant(tenantId, tenantType),
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        return role;
    }

    public ErrorOr<Success> Update(string name, Guid modifiedBy)
    {
        Guard.Against.NullOrWhiteSpace(name);

        Name = name;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> ChangePermissions(List<string> permissions, Guid modifiedBy)
    {
        Guard.Against.NullOrEmpty(permissions);

        Permissions = permissions;
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new RolePermissionsChangedEvent(Id, Tenant.TenantId));

        return Result.Success;
    }

    public ErrorOr<Success> Delete()
    {
        if (IsDeleted)
            return RoleErrors.NotFound;

        IsDeleted = true;
        ModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new RoleDeletedEvent(Id, Tenant.TenantId));

        return Result.Success;
    }

    private Role(Guid id) : base(id) { }
    protected Role() { }
}
