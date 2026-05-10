using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Accounts.Domain.AccountAggregate.Events;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Domain.AccountAggregate;

internal class Account : AggregateRoot
{
    public User User { get; private init; } = null!;
    public Tenant Tenant { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTime LastModifiedAt { get; private set; }
    public byte[] Version { get; private set; } = null!;

    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

    internal static Account Create(
        Guid userId,
        string name,
        string email,
        Guid? tenantId,
        TenantType tenantType)
    {
        Guard.Against.Default(userId);
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(email);

        if (tenantType == TenantType.Shop)
            Guard.Against.Null(tenantId);

        var account = new Account(Guid.NewGuid())
        {
            User = User.Create(userId, name, email),
            Tenant = Tenant.Create(tenantId, tenantType),
            IsDeleted = false,
            LastModifiedAt = DateTime.UtcNow
        };

        account._domainEvents.Add(new AccountCreatedEvent(account.Id, userId, email, tenantType, tenantId));

        return account;
    }

    public ErrorOr<(List<Assignment> Added, List<Assignment> Removed)> UpdateAssignments(IEnumerable<Guid> roleIds, Guid assignedBy)
    {
        if (IsDeleted)
            return AccountErrors.AlreadyDeleted;

        var incomingRoleIds = roleIds.ToHashSet();
        var currentRoleIds = _assignments.Select(a => a.RoleId).ToHashSet();

        var toRemove = currentRoleIds.Except(incomingRoleIds).ToList();
        var toAdd = incomingRoleIds.Except(currentRoleIds).ToList();

        if (toRemove.Count == 0 && toAdd.Count == 0)
            return (new List<Assignment>(), new List<Assignment>());

        var removed = new List<Assignment>();
        var roleIdSet = toRemove.ToHashSet();
        foreach (var assignment in _assignments.Where(a => roleIdSet.Contains(a.RoleId)).ToList())
        {
            _assignments.Remove(assignment);
            removed.Add(assignment);
        }

        var added = new List<Assignment>();
        foreach (var roleId in toAdd)
        {
            var assignment = Assignment.Create(roleId, assignedBy);
            _assignments.Add(assignment);
            added.Add(assignment);
        }

        LastModifiedAt = DateTime.UtcNow;

        return (added, removed);
    }



    public ErrorOr<Success> RemoveAssignment(Guid roleId)
    {
        if (IsDeleted)
            return AccountErrors.AlreadyDeleted;

        var assignment = _assignments.FirstOrDefault(a => a.RoleId == roleId);
        if (assignment is null)
            return Result.Success;

        _assignments.Remove(assignment);
        LastModifiedAt = DateTime.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> Remove()
    {
        if (IsDeleted)
            return AccountErrors.AlreadyDeleted;

        var previousRoleIds = _assignments.Select(a => a.RoleId).ToList();

        IsDeleted = true;
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new AccountRemovedEvent(Id, previousRoleIds));

        return Result.Success;
    }

    public void MarkPermissionsChanged()
    {
        LastModifiedAt = DateTime.UtcNow;
    }

    private Account(Guid id) : base(id) { }
    protected Account() { }
}
