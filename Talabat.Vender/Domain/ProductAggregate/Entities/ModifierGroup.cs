using Talabat.Vender.Core.Common;

namespace Talabat.Vender.Domain.ProductAggregate.Entities;

public class ModifierGroup : Entity
{
	public string Title { get; init; } = string.Empty;
	public int Min { get; init; }
	public int Max { get; init; }
	public ModifierGroupData Data { get; private set; }

	public ModifierGroup(
		string title,
		int min,
		int max,
		ModifierGroupData modifierGroupData
		)
	{
		Title = title;
		Min = min;
		Max = max;
		Data = modifierGroupData;
	}

	public ModifierGroup()
	{
	}
}

public class ModifierGroupItem
{
	public int ModifierId { get; init; }
	public List<int> GroupIds { get; init; } = new();
}

public class ModifierGroupData : ValueObject
{
	public List<ModifierGroupItem> ModifierGroupItems { get; init; } = new();

	public ModifierGroupData(List<ModifierGroupItem> modifierGroupItems)
	{
		ModifierGroupItems = modifierGroupItems;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ModifierGroupItems;
	}
}
