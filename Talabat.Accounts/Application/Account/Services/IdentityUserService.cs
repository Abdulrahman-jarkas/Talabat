using System.Net.Http.Json;

namespace Talabat.Accounts.Application.Account.Services;

internal class IdentityUserService(IHttpClientFactory httpClientFactory) : IIdentityUserService
{
    public async Task<IdentityUserResponse?> GetUserAsync(string userId, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("identity.api");

        var response = await client.GetAsync($"/api/users/{userId}", ct);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
            return await response.Content.ReadFromJsonAsync<IdentityUserResponse>(ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return null;
    }
}
