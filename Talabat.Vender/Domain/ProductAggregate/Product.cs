using Talabat.Vender.Core.Common;
using Talabat.Vender.Domain.ProductAggregate.Entities;

namespace Talabat.Vender.Domain.ProductAggregate;

public class Product : AggregateRoot
{
	public int VendorId { get; init; }
	public string Title { get; set; } = string.Empty;
	public decimal BasePrice { get; private set; }
	public Customization Customization { get; private set; }
	public int TaxId { get; private set; }

	public Product(
		int vendorId,
		string title,
		decimal basePrice,
		Customization customization,
		int taxCategoryId)
	{
		VendorId = vendorId;
		Title = title;
		BasePrice = basePrice;
		Customization = customization ?? Customization.Empty;
		TaxId = taxCategoryId;
	}

	public void AddModifierGroup(ModifierGroup modifierGroup)
	{
		Customization = Customization.AddModiferGroup(modifierGroup);
	}

	public Product()
	{
	}
}