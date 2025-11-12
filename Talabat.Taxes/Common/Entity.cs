namespace Talabat.Taxes.Common;

public abstract class Entity
{
    public int Id { get; init; }

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

    protected Entity(int id) => Id = id;

    protected Entity() { }
}