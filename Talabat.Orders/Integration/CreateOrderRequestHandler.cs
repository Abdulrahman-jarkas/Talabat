using ErrorOr;
using MediatR;
using Talabat.Orders.Contracts;

namespace Talabat.Orders.Integration;

internal class CreateOrderRequestHandler(IOrdersRepository ordersRepository) : IRequestHandler<CreateOnlineOrderRequest, ErrorOr<Guid>>
{
	async Task<ErrorOr<Guid>> IRequestHandler<CreateOnlineOrderRequest, ErrorOr<Guid>>.Handle(CreateOnlineOrderRequest request, CancellationToken cancellationToken)
	{
		var orderResult = new Order(
			request.CustomerId,
			request.MerchantId,
			Payment.Card(request.PaymentId, PaymentStatusValues.Unpaid),
			request.CheckoutSessionId,
			request.AddressId);


		foreach (var item in request.Items)
		{
			var addItemResult = orderResult.AddItem(item.ProductId, item.Quantity, item.BasePrice);
			if (addItemResult.IsError)
				return addItemResult.Errors;
		}

		await ordersRepository.AddAsync(orderResult);

		return orderResult.Id;
	}
}