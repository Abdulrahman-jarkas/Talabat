using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;

namespace Talabat.Checkout.Application.Services;

internal class ProductService(ISender sender) : IProductService
{
	public async Task<List<ProductValidationInfo>?> GetProductsForValidationAsync(
		IReadOnlyList<Guid> productIds,
		CancellationToken cancellationToken = default)
	{
		var products = await sender.Send(new ProductsQuery(productIds), cancellationToken);

		return products?.Select(p => new ProductValidationInfo(
			p.Id,
			p.BasePrice,
			p.AvailableStock,
			p.ReservedStock)).ToList();
	}

	public async Task<ErrorOr<Success>> ReserveStockAsync(
		List<(Guid ProductId, int Quantity)> items,
		CancellationToken cancellationToken = default)
	{
		var request = new ReserveStockRequest(
			items.Select(i => new ReserveStockItem(i.ProductId, i.Quantity)).ToList());

		return await sender.Send(request, cancellationToken);
	}

	public async Task<ErrorOr<Success>> ReleaseStockAsync(
		List<(Guid ProductId, int Quantity)> items,
		CancellationToken cancellationToken = default)
	{
		var request = new ReleaseStockRequest(
			items.Select(i => new ReleaseStockItem(i.ProductId, i.Quantity)).ToList());

		return await sender.Send(request, cancellationToken);
	}

	public async Task<ErrorOr<Success>> DeductStockAsync(
		List<(Guid ProductId, int Quantity)> items,
		CancellationToken cancellationToken = default)
	{
		var request = new DeductStockRequest(
			items.Select(i => new DeductStockItem(i.ProductId, i.Quantity)).ToList());

		return await sender.Send(request, cancellationToken);
	}
}
