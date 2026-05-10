using System.Security.Claims;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Talabat.Accounts.Application.Account.Queries.GetUserAccounts;
using Talabat.SharedKernal;

namespace Talabat.Accounts.Endpoints.Accounts;

[Authorize]
internal class GetMyAccountsEndpoint(ISender sender)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/accounts/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var result = await sender.Send(new GetMyAccountsQuery(userId), ct);

        var (response, statusCode) = result.ToApiResult();
        await HttpContext.Response.SendAsync(response, statusCode);
    }
}
