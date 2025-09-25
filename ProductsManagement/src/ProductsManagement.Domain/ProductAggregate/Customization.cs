using ProductsManagement.Core.Common;

namespace ProductsManagement.Domain.ProductAggregate;

public class Customization : ValueObject
{
	private readonly List<ModifierGroup> _modifierGroups = new();
	public IReadOnlyList<ModifierGroup> ModifierGroups => _modifierGroups.AsReadOnly();

	public override IEnumerable<object> GetEqualityComponents()
	{
		throw new NotImplementedException();
	}
}