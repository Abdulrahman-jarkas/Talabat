using FastEndpoints;
using MediatR;
using Talabat.Products.Authorization;
using Talabat.Products.Contracts;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Products.Endpoints.Shop.Customer;

internal class GetShopsEndpoint(ISender sender)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/shops");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(ShopsPermissions.List),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new ShopsQuery(), ct);

        await HttpContext.Response.SendAsync(ApiResponse.Ok(result), 200);
    }
}
