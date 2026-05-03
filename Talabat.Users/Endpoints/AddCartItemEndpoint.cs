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
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
}

public class AddCartItemValidator : Validator<AddCartItemRequest>
{
	public AddCartItemValidator()
	{
		RuleFor(x => x.ProductId).NotEmpty();
		RuleFor(x => x.Quantity).GreaterThan(0);
	}
}

[RequiredRole(AuthRoles.Customer)]
internal class AddCartItemEndpoint(ISender sender, IAccountContext accountContext)
	: Endpoint<AddCartItemRequest>
{
	public override void Configure()
	{
		Post("/api/cart/items");
		Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(AddCartItemRequest req, CancellationToken ct)
	{
		// Customer ID is the Account ID from the token
		var customerId = accountContext.AccountId;
		if (customerId is null)
		{
			HttpContext.Response.StatusCode = 401;
			return;
		}

		var result = await sender.Send(
			new AddCartItemCommand(customerId.Value, req.ProductId, req.Quantity), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
