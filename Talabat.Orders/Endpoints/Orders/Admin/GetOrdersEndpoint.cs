using FastEndpoints;
using MediatR;
using Talabat.Orders.Application.Order.Queries.GetOrders;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders.Admin;

public class GetOrdersRequest
{
    [QueryParam]
    public Guid? ShopId { get; set; }

    [QueryParam]
    public Guid? CustomerId { get; set; }
}

internal class GetOrdersEndpoint(ISender sender)
    : Endpoint<GetOrdersRequest>
{
    public override void Configure()
    {
        Get("/api/admin/orders");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(GetOrdersRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetOrdersQuery(req.ShopId, req.CustomerId), ct);

        await HttpContext.Response.SendAsync(ApiResponse.Ok(result), 200);
    }
}
