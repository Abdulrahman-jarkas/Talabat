using Microsoft.AspNetCore.Authorization;

namespace Talabat.SharedKernal.Authorization;

/// <summary>
/// Authorization requirement that validates the account version from the JWT
/// matches the current version in the database. Rejects stale tokens.
/// </summary>
public sealed class AccountVersionRequirement : IAuthorizationRequirement;
