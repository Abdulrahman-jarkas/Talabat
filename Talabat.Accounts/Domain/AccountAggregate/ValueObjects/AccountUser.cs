using Talabat.SharedKernal;

namespace Talabat.Accounts.Domain.AccountAggregate.ValueObjects;

internal class AccountUser : ValueObject
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    internal AccountUser(Guid userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }

    internal void UpdateName(string name)
    {
        Name = name;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return Name;
        yield return Email;
    }

    private AccountUser() { }
}
