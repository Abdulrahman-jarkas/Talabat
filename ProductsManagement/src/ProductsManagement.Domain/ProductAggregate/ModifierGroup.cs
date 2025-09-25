using ErrorOr;
using ProductsManagement.Core.Common;

namespace ProductsManagement.Domain.ProductAggregate;

public class ModifierGroup : ValueObject
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; private set; }
	public int Max { get; private set; }
	public Guid Id { get; init; } = Guid.CreateVersion7();

	private readonly List<Modifier> _modifiers = new();
	public IReadOnlyList<Modifier> Modifiers => _modifiers.AsReadOnly();

	private ModifierGroup(
		string title,
		int min,
		int max,
		List<Modifier> modifiers,
		Guid? id
		)
	{
		Title = title;
		Min = min;
		Max = max;
		Id = id ?? Guid.CreateVersion7();
	}

	public static ErrorOr<ModifierGroup> Create(string title, int min, int max, List<Modifier> modifiers, Guid? id)
	{
		return new ModifierGroup(title, min, max, modifiers, id);
	}

	public ErrorOr<ModifierGroup> AddModifier(Modifier modifier)
	{
		_modifiers.Add(modifier);
		return new ModifierGroup(Title, Min, Max, _modifiers, Id);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Id;
	}
}
