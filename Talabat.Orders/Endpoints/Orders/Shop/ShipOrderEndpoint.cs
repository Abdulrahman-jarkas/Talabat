using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.ShipOrder;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders.Shop;

public class ShipOrderRequest
{
    public Guid OrderId { get; set; }
}

public class ShipOrderValidator : Validator<ShipOrderRequest>
{
    public ShipOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}

internal class ShipOrderEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<ShipOrderRequest>
{
    public override void Configure()
    {
        Post("/api/shop/orders/{OrderId}/ship");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Ship),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(ShipOrderRequest req, CancellationToken ct)
    {
        var shopId = accountContext.TenantId!.Value;

        var result = await sender.Send(new ShipOrderCommand(shopId, req.OrderId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
