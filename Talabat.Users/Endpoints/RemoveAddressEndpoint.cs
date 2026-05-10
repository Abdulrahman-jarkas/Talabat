using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using Talabat.Users.Application.Customer.Commands.RemoveAddress;
using Talabat.Users.Authorization;

namespace Talabat.Users.Endpoints;

public class RemoveAddressRequest
{
    public Guid AddressId { get; set; }
}

public class RemoveAddressValidator : Validator<RemoveAddressRequest>
{
    public RemoveAddressValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("AddressId is required.");
    }
}

internal class RemoveAddressEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<RemoveAddressRequest>
{
    public override void Configure()
    {
        Delete("/api/customer/addresses/{AddressId}");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(UsersPermissions.Update),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(RemoveAddressRequest req, CancellationToken ct)
    {
        var customerId = accountContext.AccountId!.Value;

        var result = await sender.Send(new RemoveAddressCommand(customerId, req.AddressId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
