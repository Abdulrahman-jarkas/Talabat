using Talabat.Vender.Core.Common;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Domain.ProductAggregate;

public class Product : AggregateRoot
{
	public string Title { get; set; } = string.Empty;
	public decimal BasePrice { get; private set; }
	public Customization Customization { get; private set; }
	public int TaxCategoryId { get; private set; }

	public Product(
		string title,
		decimal basePrice,
		Customization customization,
		int taxCategoryId)
	{
		Title = title;
		BasePrice = basePrice;
		Customization = customization ?? Customization.Empty;
		TaxCategoryId = taxCategoryId;
	}

	public void AddModifierGroup(ModifierGroup modifierGroup)
	{
		Customization = Customization.AddModiferGroup(modifierGroup);
	}

	public Product()
	{
	}
}