namespace Talabat.Checkout.Application.Services;

internal record ProductDetails(Guid ProductId, decimal BasePrice, int Quantity);

internal interface IProductService
{
	Task<List<ProductDetails>?> GetProductDetailsAsync(
		IReadOnlyList<Guid> productIds,
		CancellationToken cancellationToken = default);
}
