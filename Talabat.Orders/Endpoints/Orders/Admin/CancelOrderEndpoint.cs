using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.Orders.Application.Order.Commands.CancelOrder;
using Talabat.Orders.Authorization;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Orders.Endpoints.Orders.Admin;

public class CancelOrderRequest
{
    public Guid OrderId { get; set; }
}

public class CancelOrderValidator : Validator<CancelOrderRequest>
{
    public CancelOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}

internal class CancelOrderEndpoint(ISender sender)
    : Endpoint<CancelOrderRequest>
{
    public override void Configure()
    {
        Post("/api/admin/orders/{OrderId}/cancel");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(OrdersPermissions.Cancel),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.System));
    }

    public override async Task HandleAsync(CancelOrderRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new CancelOrderCommand(req.OrderId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
