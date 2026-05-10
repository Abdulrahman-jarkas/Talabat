using ErrorOr;
using MediatR;
using Talabat.Accounts.Data.Repositories;

namespace Talabat.Accounts.Application.Role.Queries.GetRoles;

internal class GetRolesQueryHandler(IRolesRepository rolesRepository)
    : IRequestHandler<GetRolesQuery, ErrorOr<List<RoleDto>>>
{
    public async Task<ErrorOr<List<RoleDto>>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
    {
        var tenantTypeStr = query.TenantType?.ToString();
        var roles = await rolesRepository.GetByTenantReadOnlyAsync(tenantTypeStr, query.TenantId, cancellationToken);

        var dtos = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Permissions,
            r.Tenant.TenantId,
            r.Tenant.TenantType.ToString(),
            Convert.ToBase64String(r.Version),
            r.IsDefault,
            r.CreatedBy,
            r.CreatedAt,
            r.ModifiedBy,
            r.ModifiedAt
        )).ToList();

        return dtos;
    }
}
