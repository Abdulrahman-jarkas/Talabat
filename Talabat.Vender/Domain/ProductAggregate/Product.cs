using Talabat.Vender.Core.Common;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Domain.ProductAggregate;

public class Product : AggregateRoot
{
	public string Title { get; set; } = string.Empty;
	public decimal BasePrice { get; private set; }
	public List<ModifierGroup> ModifierGroups { get; private set; }

	public Product(
		string title,
		decimal basePrice,
		List<ModifierGroup> modifierGroups
		)
	{
		Title = title;
		BasePrice = basePrice;
		ModifierGroups = modifierGroups;
	}

	public void AddModifierGroup(ModifierGroup modifierGroup)
	{
		ModifierGroups.Add(modifierGroup);
	}

	public Product()
	{
	}
}