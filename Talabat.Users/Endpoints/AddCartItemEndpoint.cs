using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using Talabat.Users.Application.Customer.Commands.AddCartItem;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

namespace Talabat.Users.Endpoints;

public class AddCartItemRequest
{
	public Guid CustomerId { get; set; }
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
}

public class AddCartItemValidator : Validator<AddCartItemRequest>
{
	public AddCartItemValidator()
	{
		RuleFor(x => x.CustomerId).NotEmpty();
		RuleFor(x => x.ProductId).NotEmpty();
		RuleFor(x => x.Quantity).GreaterThan(0);
	}
}

[RequiredRole(AuthRoles.Customer)]
internal class AddCartItemEndpoint(ISender sender)
	: Endpoint<AddCartItemRequest>
{
	public override void Configure()
	{
		Post("/api/cart/items");
		Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(AddCartItemRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new AddCartItemCommand(req.CustomerId, req.ProductId, req.Quantity), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
