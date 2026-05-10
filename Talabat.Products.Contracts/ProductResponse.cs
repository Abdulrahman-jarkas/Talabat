namespace Talabat.Products.Contracts;

public record ProductResponse(
	Guid Id,
	string Title,
	Guid ShopId,
	decimal BasePrice,
	int Quantity
);
