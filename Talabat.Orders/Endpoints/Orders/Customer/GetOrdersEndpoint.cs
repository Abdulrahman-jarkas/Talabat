using FastEndpoints;
using MediatR;
using Talabat.Orders.Application.Order.Queries.GetOrders;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders.Customer;

internal class GetOrdersEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/orders");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customerId = accountContext.AccountId!.Value;

        var result = await sender.Send(new GetOrdersQuery(CustomerId: customerId), ct);

        await HttpContext.Response.SendAsync(ApiResponse.Ok(result), 200);
    }
}
