using ErrorOr;
using FastEndpoints;
using Talabat.OrderProcessing.Application.Services;

namespace Talabat.OrderProcessing.Endpoints.CashPay;

public class PayRequest
{
	public int OrderId { get; set; }
}

public class CashPay(IOrderService orderService) : Endpoint<PayRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/orders/cash-pay");
		AllowAnonymous();
	}

	public override Task HandleAsync(PayRequest req, CancellationToken ct)
	{
		return orderService.CashPay(req.OrderId, 0, ct);
	}
}
