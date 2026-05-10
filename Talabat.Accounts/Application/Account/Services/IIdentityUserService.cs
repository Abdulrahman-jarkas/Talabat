namespace Talabat.Accounts.Application.Account.Services;

public interface IIdentityUserService
{
    Task<IdentityUserResponse?> GetUserAsync(string userId, CancellationToken ct = default);
}
