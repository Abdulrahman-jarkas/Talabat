using Talabat.Vender.Core.Common;

namespace Talabat.Vender.Domain.ProductAggregate.Entities;

public class ModifierGroup : ValueObject
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; init; } = string.Empty;
	public int Min { get; init; }
	public int Max { get; init; }
	public List<ModifierGroupItem> Items { get; init; } = new();

	public ModifierGroup(
		string title,
		int min,
		int max,
		List<ModifierGroupItem> items
		)
	{
		Title = title;
		Min = min;
		Max = max;
		Items = items;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Id;
	}
}

public class ModifierGroupItem : ValueObject
{
	public int ModifierId { get; init; }
	public List<ModifierSubGroup> SubGroups { get; init; } = new();

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ModifierId;
		yield return SubGroups;
	}
}

public class ModifierSubGroup : ValueObject
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Title { get; set; } = string.Empty;
	public int Min { get; set; }
	public int Max { get; set; }
	public List<int> ModifierIds { get; init; } = new();

	public ModifierSubGroup(List<int> modifierIds)
	{
		ModifierIds = modifierIds;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return Id;
	}
}
