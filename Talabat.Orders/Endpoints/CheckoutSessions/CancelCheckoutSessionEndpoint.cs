using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.CheckoutSession.Commands.CancelCheckoutSession;
using Talabat.SharedKernal;

namespace Talabat.Orders.Endpoints.CheckoutSessions;

public class CancelCheckoutSessionRequest
{
	public Guid CheckoutSessionId { get; set; }
}

public class CancelCheckoutSessionValidator : Validator<CancelCheckoutSessionRequest>
{
	public CancelCheckoutSessionValidator()
	{
		RuleFor(x => x.CheckoutSessionId).NotEmpty();
	}
}

internal class CancelCheckoutSessionEndpoint(ISender sender)
	: Endpoint<CancelCheckoutSessionRequest>
{
	public override void Configure()
	{
		Post("/api/checkout-sessions/cancel");
		AllowAnonymous();
	}

	public override async Task HandleAsync(CancelCheckoutSessionRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new CancelCheckoutSessionCommand(req.CheckoutSessionId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
