using ErrorOr;
using MediatR;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Application.Account.Queries.GetAccounts;

internal record GetAccountsQuery(TenantType? TenantType, Guid? TenantId) : IRequest<ErrorOr<List<AccountDto>>>;

internal record AccountDto(
    Guid Id,
    Guid UserId,
    string Name,
    string Email,
    Guid? TenantId,
    string TenantType,
    string Version,
    List<AssignmentDto> Assignments);

internal record AssignmentDto(Guid Id, Guid RoleId, string? RoleName, Guid AssignedBy, DateTime AssignedAt);
