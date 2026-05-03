using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Role.Queries.GetRoles;

internal record GetRolesQuery(Guid? TenantId) : IRequest<ErrorOr<List<RoleDto>>>;

internal record RoleDto(
    Guid Id,
    string Name,
    List<string> Permissions,
    Guid? TenantId,
    string TenantType,
    Guid CreatedBy,
    DateTime CreatedAt,
    Guid? ModifiedBy,
    DateTime? ModifiedAt);
