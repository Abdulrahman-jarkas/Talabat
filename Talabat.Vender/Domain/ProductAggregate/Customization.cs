using ErrorOr;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Domain.ProductAggregate;

public class Customization
{
	public List<ModifierGroup> ModifierGroups { get; set; }

	public Customization(List<ModifierGroup> modifierGroups)
	{
		ModifierGroups = modifierGroups;
	}

	public ErrorOr<Success> AddModifierGroup(ModifierGroup modifierGroup)
	{
		// validation
		ModifierGroups.Add(modifierGroup);
		return Result.Success;
	}
}