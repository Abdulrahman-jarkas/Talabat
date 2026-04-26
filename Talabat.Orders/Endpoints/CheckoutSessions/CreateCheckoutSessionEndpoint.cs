using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.CheckoutSession.Commands.CreateCheckoutSession;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

namespace Talabat.Orders.Endpoints.CheckoutSessions;

public class CreateCheckoutSessionRequest
{
	public Guid CustomerId { get; set; }
}

public class CreateCheckoutSessionValidator : Validator<CreateCheckoutSessionRequest>
{
	public CreateCheckoutSessionValidator()
	{
		RuleFor(x => x.CustomerId).NotEmpty();
	}
}

[RequiredRole(AuthRoles.Customer)]
//[EnforcePlanLimit(Features.OrdersPerDay)]
internal class CreateCheckoutSessionEndpoint(ISender sender)
	: Endpoint<CreateCheckoutSessionRequest>
{
	public override void Configure()
	{
		Post("/api/checkout-sessions");
		Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(CreateCheckoutSessionRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new CreateCheckoutSessionCommand(req.CustomerId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
