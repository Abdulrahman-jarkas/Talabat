using Ardalis.GuardClauses;

namespace Talabat.Accounts.Domain.AccountAggregate;

internal class Assignment
{
    public Guid Id { get; private init; }
    public Guid RoleId { get; private init; }
    public Guid AssignedBy { get; private init; }
    public DateTime AssignedAt { get; private init; }

    internal static Assignment Create(Guid roleId, Guid assignedBy)
    {
        Guard.Against.Default(roleId);
        Guard.Against.Default(assignedBy);

        return new Assignment
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow
        };
    }

    private Assignment() { }
}
