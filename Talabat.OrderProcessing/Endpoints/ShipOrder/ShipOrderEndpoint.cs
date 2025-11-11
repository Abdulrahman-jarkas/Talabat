using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;
using Talabat.OrderProcessing.Endpoints.CreateOrder;

namespace Talabat.OrderProcessing.Endpoints.ShipOrder;

public class ShipOrderEndpoint(IOrderService orderService) : Endpoint<ShipOrderRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/orders/ship");
		AllowAnonymous();
	}

	public override Task HandleAsync(ShipOrderRequest req, CancellationToken ct)
	{
		return orderService.Ship(req.OrderId, ct);
	}
}
