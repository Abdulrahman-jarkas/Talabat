using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.Accounts.Domain.AccountAggregate.Events;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate;

internal class Account : AggregateRoot
{
    public AccountUser User { get; private set; } = null!;
    public Tenant Tenant { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTime LastModifiedAt { get; private set; }
    public byte[] Version { get; private set; } = null!;

    private readonly List<AccountRole> _accountRoles = new();
    public IReadOnlyCollection<AccountRole> AccountRoles => _accountRoles.AsReadOnly();

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
            User = new AccountUser(userId, name, email),
            Tenant = new Tenant(tenantId, tenantType),
            IsDeleted = false,
            LastModifiedAt = DateTime.UtcNow
        };

        account._domainEvents.Add(new AccountCreatedEvent(account.Id, userId, tenantId));

        return account;
    }

    public ErrorOr<Success> UpdateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);

        if (IsDeleted)
            return AccountErrors.AlreadyDeleted;

        User.UpdateName(name);
        LastModifiedAt = DateTime.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> SetRoles(IEnumerable<Guid> roleIds, Guid assignedBy)
    {
        if (IsDeleted)
            return AccountErrors.AlreadyDeleted;

        _accountRoles.Clear();

        foreach (var roleId in roleIds)
            _accountRoles.Add(AccountRole.Create(roleId, assignedBy));

        LastModifiedAt = DateTime.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> Remove()
    {
        if (IsDeleted)
            return AccountErrors.AlreadyDeleted;

        IsDeleted = true;
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new AccountRemovedEvent(Id, User.UserId));

        return Result.Success;
    }

    public void MarkPermissionsChanged()
    {
        LastModifiedAt = DateTime.UtcNow;
    }

    private Account(Guid id) : base(id) { }
    protected Account() { }
}
