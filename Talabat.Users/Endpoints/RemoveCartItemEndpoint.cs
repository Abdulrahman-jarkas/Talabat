using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.SharedKernal;
using Talabat.Users.Application.Customer.Commands.RemoveCartItem;

namespace Talabat.Users.Endpoints;

public class RemoveCartItemRequest
{
	public Guid CustomerId { get; set; }
	public Guid ProductId { get; set; }
}

public class RemoveCartItemValidator : Validator<RemoveCartItemRequest>
{
	public RemoveCartItemValidator()
	{
		RuleFor(x => x.CustomerId).NotEmpty();
		RuleFor(x => x.ProductId).NotEmpty();
	}
}

internal class RemoveCartItemEndpoint(ISender sender)
	: Endpoint<RemoveCartItemRequest>
{
	public override void Configure()
	{
		Delete("/api/cart/items");
		AllowAnonymous();
	}

	public override async Task HandleAsync(RemoveCartItemRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new RemoveCartItemCommand(req.CustomerId, req.ProductId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
