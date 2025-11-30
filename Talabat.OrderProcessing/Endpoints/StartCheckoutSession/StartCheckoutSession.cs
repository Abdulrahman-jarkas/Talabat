using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;
using Talabat.OrderProcessing.Endpoints.CreateOrder;

namespace Talabat.OrderProcessing.Endpoints.StartCheckoutSession;


public class StartCheckoutSessionEndpoint(IOrderService orderService) : Endpoint<CreateOrderRequest, ErrorOr<string>>
{
	public override void Configure()
	{
		Post("/api/start-checkout-session");
		AllowAnonymous();
	}

	public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
	{
		var res = await orderService.StartCheckoutSession(req);
		await Send.OkAsync(res);
	}
}
