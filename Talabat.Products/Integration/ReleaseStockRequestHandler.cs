using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Integration;

internal class ReleaseStockRequestHandler(IProductsRepository productsRepository)
	: IRequestHandler<ReleaseStockRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(ReleaseStockRequest request, CancellationToken cancellationToken)
	{
		var productIds = request.Items.Select(i => i.ProductId).ToList();
		var products = await productsRepository.GetProductsByIdsAsync(productIds, cancellationToken);

		foreach (var item in request.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null)
				continue; // Product may have been deleted; skip gracefully

			var result = product.ReleaseStock(item.Quantity);
			if (result.IsError)
				continue; // Best-effort release
		}

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
