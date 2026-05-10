using ErrorOr;
using MediatR;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Application.Role.Queries.GetRoles;

internal record GetRolesQuery(TenantType? TenantType, Guid? TenantId) : IRequest<ErrorOr<List<RoleDto>>>;

internal record RoleDto(
    Guid Id,
    string Name,
    IReadOnlyList<string> Permissions,
    Guid? TenantId,
    string TenantType,
    string Version,
    bool IsDefault,
    Guid CreatedBy,
    DateTime CreatedAt,
    Guid? ModifiedBy,
    DateTime? ModifiedAt);
