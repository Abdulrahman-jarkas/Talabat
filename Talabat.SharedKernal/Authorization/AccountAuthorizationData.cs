namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization data loaded from the Accounts module for the current request.
/// </summary>
public sealed record AccountAuthorizationData
{
    public required Guid AccountId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid? TenantId { get; init; }
    public required string TenantType { get; init; }
    public required string AccountName { get; init; }
    public required bool IsDeleted { get; init; }
    public required byte[] Version { get; init; }
    public required IReadOnlySet<string> Roles { get; init; }
    public required IReadOnlySet<string> Permissions { get; init; }
}
