using ProductsManagement.Core.Common;

namespace ProductsManagement.Domain.ProductAggregate;

public class Product : AggregateRoot
{
	public string Title { get; set; } = string.Empty;
	public double BasePrice { get; private set; }
	public Customization? Customization { get; private set; } 

	public Product(
		string title,
		double basePrice,
		Customization? customization
		)
	{
		Title = title;
		BasePrice = basePrice;

		if (customization is not null)
			Customization = customization;
	}
}