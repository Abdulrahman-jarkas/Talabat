using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Queries.GetUserAccounts;

internal record GetUserAccountsQuery(
    Guid UserId,
    string? TenantType,
    Guid? TenantId) : IRequest<ErrorOr<List<UserAccountDto>>>;

internal record UserAccountDto(
    Guid Id,
    string Name,
    string TenantType,
    Guid? TenantId,
    string Version,
    List<UserAccountRoleDto> Roles);

internal record UserAccountRoleDto(Guid RoleId, string? RoleName, IReadOnlyList<string> Permissions);
