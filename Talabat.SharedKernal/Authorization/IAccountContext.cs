namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Provides access to the current account's authorization context for the request.
/// Combines identity (from JWT claims) with authorization data (loaded from the Accounts module).
/// </summary>
public interface IAccountContext
{
    /// <summary>
    /// Gets the user ID from the "sub" claim.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the account ID from the "account_id" claim.
    /// </summary>
    Guid? AccountId { get; }

    /// <summary>
    /// Gets the tenant type from the account authorization data ("Customer" | "Shop" | "System").
    /// </summary>
    string? TenantType { get; }

    /// <summary>
    /// Gets the tenant ID from the account authorization data.
    /// Returns null for customer/system tenant types.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Gets the account display name.
    /// </summary>
    string? AccountName { get; }

    /// <summary>
    /// Checks if the account has the specified permission.
    /// </summary>
    bool HasPermission(string permission);

    /// <summary>
    /// Checks if the account has the specified role.
    /// </summary>
    bool HasRole(string role);

    /// <summary>
    /// Gets the account_version claim value from the JWT token.
    /// </summary>
    string? GetTokenVersion();

    /// <summary>
    /// Gets the full authorization data loaded from the Accounts module.
    /// Returns null if no account_id claim or account not found.
    /// </summary>
    Task<AccountAuthorizationData?> GetAuthorizationDataAsync(CancellationToken cancellationToken = default);
}
