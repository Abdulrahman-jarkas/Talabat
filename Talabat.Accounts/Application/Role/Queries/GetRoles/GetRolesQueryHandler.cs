using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;

namespace Talabat.Accounts.Application.Role.Queries.GetRoles;

internal class GetRolesQueryHandler(IRolesRepository rolesRepository)
    : IRequestHandler<GetRolesQuery, ErrorOr<List<RoleDto>>>
{
    public async Task<ErrorOr<List<RoleDto>>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
    {
        var roles = await rolesRepository.GetByTenantAsync(query.TenantId, cancellationToken);

        var dtos = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Permissions,
            r.Tenant.TenantId,
            r.Tenant.TenantType.ToString(),
            r.CreatedBy,
            r.CreatedAt,
            r.ModifiedBy,
            r.ModifiedAt
        )).ToList();

        return dtos;
    }
}
