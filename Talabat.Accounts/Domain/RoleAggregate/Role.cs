using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.Accounts.Domain.RoleAggregate.Events;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Domain.RoleAggregate;

internal class Role : AggregateRoot
{
    public string Name { get; private set; } = null!;

    private List<string> _permissions = new();
    public IReadOnlyList<string> Permissions => _permissions.AsReadOnly();

    public Tenant Tenant { get; private set; } = null!;
    public bool IsDefault { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public byte[] Version { get; private set; } = null!;

    internal static ErrorOr<Role> Create(
        string name,
        List<string> permissions,
        Guid? tenantId,
        TenantType tenantType,
        Guid createdBy,
        bool isDefault = false)
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
            _permissions = permissions,
            Tenant = Tenant.Create(tenantId, tenantType),
            IsDefault = isDefault,
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

        _permissions = permissions;
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

    public void MarkAsDefault()
    {
        IsDefault = true;
    }

    private Role(Guid id) : base(id) { }
    protected Role() { }
}
