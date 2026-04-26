using Talabat.SharedKernal;

namespace Talabat.Products.Domain;

/// <summary>
/// Represents a shop in the system (for testing purposes).
/// </summary>
internal class Shop : Entity
{
    public string Name { get; private set; } = string.Empty;

    internal Shop(Guid id, string name) : base(id)
    {
        Name = name;
    }

    private Shop() { }
}
