using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using Talabat.Users.Application.Customer.Commands.RemoveCartItem;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

namespace Talabat.Users.Endpoints;

public class RemoveCartItemRequest
{
	public Guid ProductId { get; set; }
}

public class RemoveCartItemValidator : Validator<RemoveCartItemRequest>
{
	public RemoveCartItemValidator()
	{
		RuleFor(x => x.ProductId).NotEmpty();
	}
}

[RequiredRole(AuthRoles.Customer)]
internal class RemoveCartItemEndpoint(ISender sender, IAccountContext accountContext)
	: Endpoint<RemoveCartItemRequest>
{
	public override void Configure()
	{
		Delete("/api/cart/items");
		Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
	}

	public override async Task HandleAsync(RemoveCartItemRequest req, CancellationToken ct)
	{
		// Customer ID is the Account ID from the token
		var customerId = accountContext.AccountId;
		if (customerId is null)
		{
			HttpContext.Response.StatusCode = 401;
			return;
		}

		var result = await sender.Send(
			new RemoveCartItemCommand(customerId.Value, req.ProductId), ct);

		var (response, statusCode) = result.ToApiResult();
		await HttpContext.Response.SendAsync(response, statusCode);
	}
}
