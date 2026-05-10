using FastEndpoints;
using MediatR;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using Talabat.Users.Authorization;
using Talabat.Users.Contracts;

namespace Talabat.Users.Endpoints;

internal class GetCustomerDetailsEndpoint(ISender sender, IAccountContext accountContext)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/customer/details");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(UsersPermissions.Read),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customerId = accountContext.AccountId!.Value;

        var result = await sender.Send(new CustomerDetailsQuery(customerId), ct);

        if (result is null)
        {
            await HttpContext.Response.SendAsync(ApiResponse.Fail<CustomerDetailsResponse?>("Customer details not found."), 404);
            return;
        }

        await HttpContext.Response.SendAsync(ApiResponse.Ok<CustomerDetailsResponse?>(result), 200);
    }
}
