using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

/// <summary>
/// Represents a shop in the system (for testing purposes).
/// </summary>
internal class Shop : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }

    internal Shop(string name, string description)
    {
        Name = name;
        Description = description;
    }

    internal Shop(Guid id, string name) : base(id)
    {
        Name = name;
    }

    internal ErrorOr<Success> Update(string name, string description)
    {
        Name = name;
        Description = description;
        return Result.Success;
    }

    internal ErrorOr<Success> SoftDelete()
    {
        IsDeleted = true;
        return Result.Success;
    }

    private Shop() { }
}
