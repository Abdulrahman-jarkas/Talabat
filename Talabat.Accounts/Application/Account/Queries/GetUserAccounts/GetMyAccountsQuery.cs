using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Queries.GetUserAccounts;

internal record GetMyAccountsQuery(
    Guid UserId) : IRequest<ErrorOr<List<MyAccountDto>>>;

internal record MyAccountDto(
    Guid Id,
    string Name,
    string TenantType,
    Guid? TenantId,
    string Version,
    List<MyAccountRoleDto> Roles);

internal record MyAccountRoleDto(Guid RoleId, string? RoleName, IReadOnlyList<string> Permissions);
