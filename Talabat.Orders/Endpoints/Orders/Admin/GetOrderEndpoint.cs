using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Queries.GetOrder;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders.Admin;

public class GetOrderRequest
{
    public Guid OrderId { get; set; }
}

public class GetOrderValidator : Validator<GetOrderRequest>
{
    public GetOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}

internal class GetOrderEndpoint(ISender sender)
    : Endpoint<GetOrderRequest>
{
    public override void Configure()
    {
        Get("/api/admin/orders/{OrderId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(GetOrderRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetOrderQuery(req.OrderId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
