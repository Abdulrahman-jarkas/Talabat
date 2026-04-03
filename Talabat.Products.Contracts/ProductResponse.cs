namespace Talabat.Products.Contracts;

public record ProductResponse(
	Guid Id,
	string Title,
	Guid Merchant,
	decimal BasePrice,
	int Quantity
);
