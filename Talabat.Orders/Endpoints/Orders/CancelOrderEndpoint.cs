using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.CancelOrder;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

namespace Talabat.Orders.Endpoints.Orders;

public class CancelOrderRequest
{
	public Guid OrderId { get; set; }
}

public class CancelOrderValidator : Validator<CancelOrderRequest>
{
	public CancelOrderValidator()
	{
		RuleFor(x => x.OrderId).NotEmpty();
	}
}

[RequiredPermission(OrdersPermissions.Cancel)]
[RequiredRole(AuthRoles.Customer)]
internal class CancelOrderEndpoint(ISender sender)
	: Endpoint<CancelOrderRequest>
{
	public override void Configure()
	{
		Post("/api/orders/cancel");
		Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Cancel),
		         AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(CancelOrderRequest req, CancellationToken ct)
	{
		var result = await sender.Send(new CancelOrderCommand(req.OrderId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
