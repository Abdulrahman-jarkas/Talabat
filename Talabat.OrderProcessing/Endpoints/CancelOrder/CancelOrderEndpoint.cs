using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;

namespace Talabat.OrderProcessing.Endpoints.CancelOrder;

public class CancelOrderEndpoint(IOrderService orderService) : Endpoint<CancelOrderRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/orders/cancel");
		AllowAnonymous();
	}

	public override Task HandleAsync(CancelOrderRequest req, CancellationToken ct)
	{
		return orderService.Cancel(req.OrderId, ct);
	}
}
