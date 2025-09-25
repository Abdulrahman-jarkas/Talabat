using ErrorOr;
using ProductsManagement.Core.Common;

namespace ProductsManagement.Domain.ProductAggregate;

public class Modifier : ValueObject
{
	public string Title { get; private set; } = string.Empty;
	public double Price { get; private set; }
	public Guid Id { get; init; } = Guid.CreateVersion7();

	private readonly List<ModifierGroup> _modifierGroups = new();
	public IReadOnlyList<ModifierGroup> ModifierGroups => _modifierGroups.AsReadOnly();

	private Modifier(
		string title,
		double price,
		List<ModifierGroup> modifierGroups,
		Guid? id
		)
	{
		Title = title;
		Price = price;
		_modifierGroups = modifierGroups;
		Id = id ?? Guid.CreateVersion7();
	}

	public static ErrorOr<Modifier> Create(string title, double price, List<ModifierGroup> modifierGroups, Guid? id)
	{
		return new Modifier(title, price, modifierGroups, id);
	}
	
	public ErrorOr<Modifier> AddModifierGroup(ModifierGroup modifierGroup)
	{
		_modifierGroups.Add(modifierGroup);
		return new Modifier(Title, Price, _modifierGroups, Id);
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Id;
	}
}
