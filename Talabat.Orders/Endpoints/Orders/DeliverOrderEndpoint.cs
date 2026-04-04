using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.DeliverOrder;
using Talabat.SharedKernal;

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

internal class DeliverOrderEndpoint(ISender sender)
	: Endpoint<DeliverOrderRequest>
{
	public override void Configure()
	{
		Post("/api/orders/deliver");
		AllowAnonymous();
	}

	public override async Task HandleAsync(DeliverOrderRequest req, CancellationToken ct)
	{
		var result = await sender.Send(new DeliverOrderCommand(req.OrderId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
