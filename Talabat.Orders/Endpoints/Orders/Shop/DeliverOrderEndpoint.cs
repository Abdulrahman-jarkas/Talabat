using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.DeliverOrder;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders.Shop;

public class DeliverOrderRequest
{
    public Guid OrderId { get; set; }
}

public class DeliverOrderValidator : Validator<DeliverOrderRequest>
{
    public DeliverOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}

internal class DeliverOrderEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<DeliverOrderRequest>
{
    public override void Configure()
    {
        Post("/api/shop/orders/{OrderId}/deliver");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Deliver),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Shop));
    }

    public override async Task HandleAsync(DeliverOrderRequest req, CancellationToken ct)
    {
        var shopId = accountContext.TenantId!.Value;

        var result = await sender.Send(new DeliverOrderCommand(req.OrderId, shopId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
