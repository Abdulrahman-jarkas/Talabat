using ErrorOr;
using MediatR;
using Talabat.Products.Contracts;
using Talabat.Products.Data.Repositories;

namespace Talabat.Products.Application.Reservation.Commands.AddReservation;

internal class AddReservationRequestHandler(IProductsRepository productsRepository)
	: IRequestHandler<AddReservationRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(AddReservationRequest request, CancellationToken cancellationToken)
	{
		var productIds = request.Items.Select(i => i.ProductId).ToList();
		var products = await productsRepository.GetProductsByIdsAsync(productIds, cancellationToken);

		foreach (var item in request.Items)
		{
			var product = products.FirstOrDefault(p => p.Id == item.ProductId);
			if (product is null)
				return Domain.ProductErrors.NotFound(item.ProductId);

			var result = product.AddReservation(item.CheckoutSessionId, item.UserId, item.Quantity);
			if (result.IsError)
				return result.Errors;
		}

		await productsRepository.SaveChangesAsync();

		return Result.Success;
	}
}
