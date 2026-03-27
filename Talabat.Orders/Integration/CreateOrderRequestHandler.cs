using ErrorOr;
using MediatR;
using Talabat.Orders.Contracts;
using Talabat.Orders.Data.Repositories;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Integration;

internal class CreateOrderRequestHandler(IOrdersRepository ordersRepository)
	: IRequestHandler<CreateOrderRequest, ErrorOr<CreateOrderResponse>>
{
	public async Task<ErrorOr<CreateOrderResponse>> Handle(
		CreateOrderRequest request,
		CancellationToken cancellationToken)
	{
		var payment = Payment.Create(request.PaymentId);

		var order = new Order(
			request.CustomerId,
			request.MerchantId,
			payment,
			request.CheckoutSessionId,
			request.AddressId);

		foreach (var item in request.Items)
		{
			var addItemResult = order.AddItem(item.ProductId, item.Quantity, item.BasePrice);

			if (addItemResult.IsError)
				return addItemResult.Errors;
		}

		await ordersRepository.AddAsync(order, cancellationToken);
		await ordersRepository.SaveChangesAsync(cancellationToken);

		return new CreateOrderResponse(order.Id);
	}
}
