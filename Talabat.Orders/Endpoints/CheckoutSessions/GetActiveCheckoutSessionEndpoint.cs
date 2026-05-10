using FastEndpoints;
using MediatR;
using Talabat.Orders.Application.CheckoutSession.Queries.GetActiveCheckoutSession;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using AuthRoles = Talabat.SharedKernal.Authorization.Roles;

namespace Talabat.Orders.Endpoints.CheckoutSessions;

internal class GetActiveCheckoutSessionEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/checkout-sessions/active");
        Policies(AuthorizationPolicyProvider.GetRolePolicyName(AuthRoles.Customer));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customerId = accountContext.AccountId;
        if (customerId is null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var result = await sender.Send(
            new GetActiveCheckoutSessionQuery(customerId.Value), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
