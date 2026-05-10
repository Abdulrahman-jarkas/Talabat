using FastEndpoints;
using FluentValidation;
using MediatR;
using Talabat.SharedKernal;
using Talabat.SharedKernal.Authorization;
using Talabat.Users.Application.Customer.Commands.AddAddress;
using Talabat.Users.Authorization;

namespace Talabat.Users.Endpoints;

public class AddAddressRequest
{
    public string Address { get; set; } = string.Empty;
}

public class AddAddressValidator : Validator<AddAddressRequest>
{
    public AddAddressValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
    }
}

internal class AddAddressEndpoint(ISender sender, IAccountContext accountContext)
    : Endpoint<AddAddressRequest>
{
    public override void Configure()
    {
        Post("/api/customer/addresses");
        Policies(
            AuthorizationPolicyProvider.GetPermissionPolicyName(UsersPermissions.Update),
            AuthorizationPolicyProvider.GetTenantTypePolicyName(TenantType.Customer));
    }

    public override async Task HandleAsync(AddAddressRequest req, CancellationToken ct)
    {
        var customerId = accountContext.AccountId!.Value;

        var result = await sender.Send(new AddAddressCommand(customerId, req.Address), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
