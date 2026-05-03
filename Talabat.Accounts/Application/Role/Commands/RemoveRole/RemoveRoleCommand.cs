using ErrorOr;
using MediatR;

namespace Talabat.Accounts.Application.Role.Commands.RemoveRole;

internal record RemoveRoleCommand(Guid RoleId, Guid? TenantId) : IRequest<ErrorOr<Success>>;
