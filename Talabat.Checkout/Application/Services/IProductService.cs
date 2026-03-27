using ErrorOr;

namespace Talabat.Checkout.Application.Services;

internal record ProductValidationInfo(Guid ProductId, decimal BasePrice, int AvailableStock, int ReservedStock)
{
	public int EffectiveStock => AvailableStock - ReservedStock;
}

internal interface IProductService
{
	Task<List<ProductValidationInfo>?> GetProductsForValidationAsync(
		IReadOnlyList<Guid> productIds,
		CancellationToken cancellationToken = default);

	Task<ErrorOr<Success>> ReserveStockAsync(
		List<(Guid ProductId, int Quantity)> items,
		CancellationToken cancellationToken = default);

	Task<ErrorOr<Success>> ReleaseStockAsync(
		List<(Guid ProductId, int Quantity)> items,
		CancellationToken cancellationToken = default);

	Task<ErrorOr<Success>> DeductStockAsync(
		List<(Guid ProductId, int Quantity)> items,
		CancellationToken cancellationToken = default);
}
