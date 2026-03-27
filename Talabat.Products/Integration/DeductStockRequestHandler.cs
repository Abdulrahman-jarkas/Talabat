using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;
using Talabat.Products.Domain;

namespace Talabat.Products.Integration;

internal class DeductStockRequestHandler(IProductsRepository productsRepository)
	: IRequestHandler<DeductStockRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(DeductStockRequest request, CancellationToken cancellationToken)
	{
		var productIds = request.Items.Select(i => i.ProductId).ToList();
		var products = await productsRepository.GetProductsByIdsAsync(productIds, cancellationToken);

		foreach (var item in request.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null)
				return ProductErrors.NotFound(item.ProductId);

			var result = product.DeductStock(item.Quantity);
			if (result.IsError)
				return result.Errors;
		}

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
