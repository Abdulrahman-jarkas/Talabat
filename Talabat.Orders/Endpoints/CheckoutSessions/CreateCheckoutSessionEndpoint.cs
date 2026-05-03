using FastEndpoints;
using MediatR;
using Talabat.Orders.Application.CheckoutSession.Commands.CreateCheckoutSession;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

namespace Talabat.Orders.Endpoints.CheckoutSessions;

[RequiredRole(AuthRoles.Customer)]
//[EnforcePlanLimit(Features.OrdersPerDay)]
internal class CreateCheckoutSessionEndpoint(ISender sender, IAccountContext accountContext)
	: EndpointWithoutRequest
{
	public override void Configure()
	{
		Post("/api/checkout-sessions");
		Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(CancellationToken ct)
	{
		// Customer ID is the Account ID from the token
		var customerId = accountContext.AccountId;
		if (customerId is null)
		{
			HttpContext.Response.StatusCode = 401;
			return;
		}

		var result = await sender.Send(
			new CreateCheckoutSessionCommand(customerId.Value), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
