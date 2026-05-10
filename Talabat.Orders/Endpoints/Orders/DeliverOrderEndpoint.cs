using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.DeliverOrder;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders;

public class DeliverOrderRequest
{
	public Guid OrderId { get; set; }
}

public class DeliverOrderValidator : Validator<DeliverOrderRequest>
{
	public DeliverOrderValidator()
	{
		RuleFor(x => x.OrderId).NotEmpty();
	}
}

[RequiredPermission(OrdersPermissions.Deliver)]
internal class DeliverOrderEndpoint(ISender sender)
	: Endpoint<DeliverOrderRequest>
{
	public override void Configure()
	{
		Post("/api/orders/deliver");
		Policies(AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Deliver));
	}

	public override async Task HandleAsync(DeliverOrderRequest req, CancellationToken ct)
	{
		var result = await sender.Send(new DeliverOrderCommand(req.OrderId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
