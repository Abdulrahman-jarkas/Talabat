using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.DTOs;
using Talabat.OrderProcessing.Application.Services;

namespace Talabat.OrderProcessing.Endpoints.CreateOrder;

public class CreateOrderEndpoint(IOrderService orderService) : Endpoint<CreateOrderRequest, ErrorOr<OrderDetailsDto>>
{
	public override void Configure()
	{
		Post("/api/create-order");
		AllowAnonymous();
	}

	public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
	{
		var res = await orderService.CreateOrderAsync(req);
		await Send.OkAsync(res);
	}
}