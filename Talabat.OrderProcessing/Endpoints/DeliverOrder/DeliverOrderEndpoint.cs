using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;

namespace Talabat.OrderProcessing.Endpoints.DeliverOrder;

public class DeliverOrderEndpoint(IOrderService orderService) : Endpoint<DeliverOrderRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/orders/deliver");
		AllowAnonymous();
	}

	public override Task HandleAsync(DeliverOrderRequest req, CancellationToken ct)
	{
		return orderService.Deliver(req.OrderId, ct);
	}
}