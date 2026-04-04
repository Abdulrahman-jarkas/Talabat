using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.ShipOrder;
using Talabat.SharedKernal;

namespace Talabat.Orders.Endpoints.Orders;

public class ShipOrderRequest
{
	public Guid OrderId { get; set; }
}

public class ShipOrderValidator : Validator<ShipOrderRequest>
{
	public ShipOrderValidator()
	{
		RuleFor(x => x.OrderId).NotEmpty();
	}
}

internal class ShipOrderEndpoint(ISender sender)
	: Endpoint<ShipOrderRequest>
{
	public override void Configure()
	{
		Post("/api/orders/ship");
		AllowAnonymous();
	}

	public override async Task HandleAsync(ShipOrderRequest req, CancellationToken ct)
	{
		var result = await sender.Send(new ShipOrderCommand(req.OrderId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
