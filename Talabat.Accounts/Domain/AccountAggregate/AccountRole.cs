namespace Talabat.Accounts.Domain.AccountAggregate;

internal record AccountRole
{
    public Guid RoleId { get; init; }
    public Guid AssignedBy { get; init; }
    public DateTime AssignedAt { get; init; }

    internal static AccountRole Create(Guid roleId, Guid assignedBy)
    {
        return new AccountRole
        {
            RoleId = roleId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow
        };
    }
}
