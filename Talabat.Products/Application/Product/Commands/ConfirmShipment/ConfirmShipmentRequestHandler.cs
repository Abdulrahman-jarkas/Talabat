using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;
using Talabat.SharedKernal;

namespace Talabat.Products.Application.Product.Commands.ConfirmShipment;

internal class ConfirmShipmentRequestHandler(IProductsRepository productsRepository)
	: IRequestHandler<ConfirmShipmentRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(ConfirmShipmentRequest request, CancellationToken cancellationToken)
	{
		var productIds = request.Items.Select(i => i.ProductId).ToList();
		var products = await productsRepository.GetProductsByIdsAsync(productIds, cancellationToken);

		foreach (var item in request.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);

			if (product is null)
				continue; // Product may have been soft-deleted; skip gracefully

			var result = product.ConfirmShipment(item.OrderId);
			if (result.IsError)
				throw new EventualConsistencyException(
				EventualConsistencyError.From(
					"ConfirmShipment.Failed",
					$"Failed to confirm shipment for Product {item.ProductId} in Order {item.OrderId}."),
				result.Errors);
		}

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
