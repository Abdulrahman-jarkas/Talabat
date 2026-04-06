using FastEndpoints;
using MediatR;
using Talabat.Orders.Application.CheckoutSession.Commands.Checkout;
using Talabat.SharedKernal;

namespace Talabat.Orders.Endpoints.CheckoutSessions;

public class CheckoutRequest
{
	public Guid CheckoutSessionId { get; set; }
	public Guid AddressId { get; set; }
}

public class CheckoutResponse
{
	public Guid PaymentId { get; set; }
	public string PaymentUrl { get; set; } = string.Empty;
}

internal class CheckoutEndpoint(ISender sender)
	: Endpoint<CheckoutRequest, CheckoutResponse>
{
	public override void Configure()
	{
		Post("/api/checkout-sessions/checkout");
		AllowAnonymous();
	}

	public override async Task HandleAsync(CheckoutRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new CheckoutCommand(req.CheckoutSessionId, req.AddressId), ct);

		var mapped = result.Then(r => new CheckoutResponse
		{
			PaymentId = r.PaymentId,
			PaymentUrl = r.PaymentUrl
		});

		var (response, statusCode) = mapped.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
