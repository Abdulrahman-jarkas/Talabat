using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Account.Queries.GetAccounts;

internal record GetAccountsQuery(Guid? TenantId) : IRequest<ErrorOr<List<AccountDto>>>;

internal record AccountDto(
    Guid Id,
    Guid UserId,
    string Name,
    string Email,
    Guid? TenantId,
    string TenantType,
    List<AccountRoleDto> Roles);

internal record AccountRoleDto(Guid RoleId, string? RoleName, Guid AssignedBy, DateTime AssignedAt);
