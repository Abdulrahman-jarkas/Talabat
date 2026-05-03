namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Provides account authorization data from the Accounts module.
/// Implemented in the Accounts module, consumed by SharedKernel authorization handlers.
/// </summary>
public interface IAccountAuthorizationDataProvider
{
    /// <summary>
    /// Loads authorization data for the given account.
    /// Returns null if the account is not found, deleted, or inactive.
    /// </summary>
    Task<AccountAuthorizationData?> GetAsync(Guid accountId, CancellationToken cancellationToken = default);
}
