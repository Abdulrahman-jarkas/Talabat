using System.Security.Claims;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Talabat.Accounts.Application.Account.Queries.GetUserAccounts;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Endpoints.Accounts;

public class GetUserAccountsRequest
{
    [QueryParam]
    public string? TenantType { get; set; }

    [QueryParam]
    public Guid? TenantId { get; set; }
}

[Authorize]
internal class GetUserAccountsEndpoint(ISender sender)
    : Endpoint<GetUserAccountsRequest>
{
    public override void Configure()
    {
        Get("/api/accounts/me");
    }

    public override async Task HandleAsync(GetUserAccountsRequest req, CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var result = await sender.Send(new GetUserAccountsQuery(
            userId, req.TenantType, req.TenantId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
