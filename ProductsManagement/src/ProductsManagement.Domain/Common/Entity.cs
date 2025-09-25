namespace ProductsManagement.Core.Common;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public override bool Equals(object? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return ((Entity)other).Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    protected Entity(Guid id) => Id = id;

    protected Entity() { }
}