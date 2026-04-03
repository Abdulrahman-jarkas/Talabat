using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Integration;

internal class EditReservationRequestHandler(IProductsRepository productsRepository)
	: IRequestHandler<EditReservationRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(EditReservationRequest request, CancellationToken cancellationToken)
	{
		var productIds = request.Items.Select(i => i.ProductId).ToList();
		var products = await productsRepository.GetProductsByIdsAsync(productIds, cancellationToken);

		foreach (var item in request.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);
			if (product is null)
				return Domain.ProductErrors.NotFound(item.ProductId);

			var result = product.EditReservation(item.CheckoutSessionId, item.OrderId);
			if (result.IsError)
				return result.Errors;
		}

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
