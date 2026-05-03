using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Scoped service that provides the current account's authorization context.
/// Reads identity from JWT claims and loads authorization data from the Accounts module once per request.
/// </summary>
public sealed class AccountContext : IAccountContext
{
    private readonly ClaimsPrincipal? _user;
    private readonly IAccountAuthorizationDataProvider _provider;
    private AccountAuthorizationData? _data;
    private bool _loaded;

    public AccountContext(IHttpContextAccessor httpContextAccessor, IAccountAuthorizationDataProvider provider)
    {
        _user = httpContextAccessor.HttpContext?.User;
        _provider = provider;
    }

    /// <inheritdoc />
    public string? UserId => _user?.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <inheritdoc />
    public Guid? AccountId => ParseGuidClaim(AuthorizationClaimTypes.AccountId);

    /// <inheritdoc />
    public string? TenantType => GetData()?.TenantType;

    /// <inheritdoc />
    public Guid? TenantId => GetData()?.TenantId;

    /// <inheritdoc />
    public string? AccountName => GetData()?.AccountName;

    /// <inheritdoc />
    public bool HasPermission(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        var data = GetData();
        return data?.Permissions.Contains(permission) ?? false;
    }

    /// <inheritdoc />
    public bool HasRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        var data = GetData();
        return data?.Roles.Contains(role) ?? false;
    }

    /// <inheritdoc />
    public string? GetTokenVersion()
    {
        return _user?.FindFirstValue(AuthorizationClaimTypes.AccountVersion);
    }

    /// <inheritdoc />
    public async Task<AccountAuthorizationData?> GetAuthorizationDataAsync(CancellationToken cancellationToken = default)
    {
        if (_loaded)
            return _data;

        var accountId = AccountId;
        if (accountId is null)
        {
            _loaded = true;
            return null;
        }

        _data = await _provider.GetAsync(accountId.Value, cancellationToken);
        _loaded = true;

        return _data;
    }

    private Guid? ParseGuidClaim(string claimType)
    {
        var value = _user?.FindFirstValue(claimType);
        if (string.IsNullOrEmpty(value))
            return null;

        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    private AccountAuthorizationData? GetData()
    {
        if (_loaded)
            return _data;

        // Block synchronously — data is already cached after first auth handler runs
        return GetAuthorizationDataAsync().GetAwaiter().GetResult();
    }
}
