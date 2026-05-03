using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.CheckoutSession.Commands.CancelCheckoutSession;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

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

[RequiredRole(AuthRoles.Customer)]
internal class CancelCheckoutSessionEndpoint(ISender sender, IAccountContext accountContext)
	: Endpoint<CancelCheckoutSessionRequest>
{
	public override void Configure()
	{
		Post("/api/checkout-sessions/cancel");
		Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(CancelCheckoutSessionRequest req, CancellationToken ct)
	{
		// Customer ID is the Account ID from the token
		var customerId = accountContext.AccountId;
		if (customerId is null)
		{
			HttpContext.Response.StatusCode = 401;
			return;
		}

		var result = await sender.Send(
			new CancelCheckoutSessionCommand(customerId.Value, req.CheckoutSessionId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
