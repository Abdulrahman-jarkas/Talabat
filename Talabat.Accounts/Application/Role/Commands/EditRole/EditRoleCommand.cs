using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Role.Commands.EditRole;

internal record EditRoleCommand(
    Guid RoleId,
    string Name,
    List<string> Permissions,
    Guid? TenantId,
    Guid ModifiedBy,
    string Version) : IRequest<ErrorOr<MutationResult>>;
