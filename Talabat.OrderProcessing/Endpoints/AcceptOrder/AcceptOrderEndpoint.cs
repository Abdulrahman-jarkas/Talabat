using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;

namespace Talabat.OrderProcessing.Endpoints.AcceptOrder;

public class AcceptOrderEndpoint(IOrderService orderService) : Endpoint<AcceptOrderRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/orders/accept");
		AllowAnonymous();
	}

	public override Task HandleAsync(AcceptOrderRequest req, CancellationToken ct)
	{
		return orderService.Accept(req.OrderId, ct);
	}
}
