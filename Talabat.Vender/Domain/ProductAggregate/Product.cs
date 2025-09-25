using ErrorOr;
using Talabat.Vender.Core.Common;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Domain.ProductAggregate;

public class Product : AggregateRoot
{
	public string Title { get; set; } = string.Empty;
	public decimal BasePrice { get; private set; }

	private readonly List<ModifierGroup> _modifierGroups = new();
	public IReadOnlyList<ModifierGroup> ModifierGroups => _modifierGroups.AsReadOnly();

	public Product(
		string title,
		decimal basePrice, 
		List<ModifierGroup> modifierGroups
		)
	{
		Title = title;
		BasePrice = basePrice;
		_modifierGroups = modifierGroups;
	}

	public ErrorOr<Success> AddModifierGroup(ModifierGroup modifierGroup)
	{
		_modifierGroups.Add(modifierGroup);
		return Result.Success;
	}

	public Product()
	{
	}
}