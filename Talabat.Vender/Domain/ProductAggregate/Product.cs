using Talabat.Vender.Core.Common;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Domain.ProductAggregate;

public class Product : AggregateRoot
{
	public string Title { get; set; } = string.Empty;
	public decimal BasePrice { get; private set; }
	public Customization Customization { get; private set; }

	public Product(
		string title,
		decimal basePrice,
		Customization customization
		)
	{
		Title = title;
		BasePrice = basePrice;
		Customization = customization ?? Customization.Empty;
	}

	public void AddModifierGroup(ModifierGroup modifierGroup)
	{
		Customization = Customization.AddModiferGroup(modifierGroup);
	}

	public Product()
	{
	}
}

public class Customization : ValueObject
{
	public List<ModifierGroup> ModifierGroups { get; private set; }

	public static readonly Customization Empty = new Customization(new List<ModifierGroup>());

	private Customization(List<ModifierGroup> modifierGroups)
	{
		ModifierGroups = modifierGroups;
	}

	public static Customization Create(List<ModifierGroup> modifierGroups)
	{
		return new Customization(modifierGroups);
	}

	public Customization AddModiferGroup(ModifierGroup modifierGroup)
	{
		ModifierGroups.Add(modifierGroup);
		return this;
	}

	public override IEnumerable<object> GetEqualityComponents()
	{
		yield return ModifierGroups;
	}
}