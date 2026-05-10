using Ardalis.GuardClauses;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate.ValueObjects;

internal class User : ValueObject
{
    public Guid UserId { get; private init; }
    public string Name { get; private init; } = null!;
    public string Email { get; private init; } = null!;

    internal static User Create(Guid userId, string name, string email)
    {
        Guard.Against.Default(userId);
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(email);

        return new User
        {
            UserId = userId,
            Name = name,
            Email = email
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return Name;
        yield return Email;
    }

    private User() { }
}
