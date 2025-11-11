using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;

namespace Talabat.OrderProcessing.Endpoints.RejectOrder;

public class RejectOrderEndpoint(IOrderService orderService) : Endpoint<RejectOrderRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/orders/reject");
		AllowAnonymous();
	}

	public override Task HandleAsync(RejectOrderRequest req, CancellationToken ct)
	{
		return orderService.Cancel(req.OrderId, ct);
	}
}
