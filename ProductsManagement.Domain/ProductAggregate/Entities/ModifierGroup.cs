using ErrorOr;
using ProductsManagement.Core.Common;

namespace ProductsManagement.Domain.ProductAggregate.Entities;

public class ModifierGroup : Entity
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; private set; }
	public int Max { get; private set; }

	private readonly List<Guid> _modifierIds = new();
	public IReadOnlyList<Guid> ModifierIds => _modifierIds.AsReadOnly();

	public ModifierGroup(
		string title,
		int min,
		int max,
		List<Guid> modifierIds
		)
	{
		Title = title;
		Min = min;
		Max = max;
		_modifierIds = modifierIds;
	}

	public ErrorOr<Success> AddModifier(Guid modifier)
	{
		_modifierIds.Add(modifier);
		return Result.Success;
	}

	public ModifierGroup()
	{
	}
}
