using ErrorOr;
using ProductsManagement.Core.Common;

namespace ProductsManagement.Domain.ProductAggregate.Entities;

public class Modifier : Entity
{
	public string Title { get; private set; } = string.Empty;
	public decimal Price { get; private set; }

	private readonly List<Guid> _modifierGroupIds = new();
	public IReadOnlyList<Guid> ModifierGroupIds => _modifierGroupIds.AsReadOnly();

	private Modifier(
		string title,
		decimal price,
		List<Guid> modifierGroupIds
		)
	{
		Title = title;
		Price = price;
		_modifierGroupIds = modifierGroupIds;
	}

	public static ErrorOr<Modifier> Create(string title, decimal price, List<Guid> modifierGroupIds)
	{
		return new Modifier(title, price, modifierGroupIds);
	}

	public ErrorOr<Modifier> AddModifierGroup(Guid modifierGroupId)
	{
		_modifierGroupIds.Add(modifierGroupId);
		return new Modifier(Title, Price, _modifierGroupIds);
	}
}
