using ErrorOr;
using MediatR;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Application.Role.Commands.AddRole;

internal record AddRoleCommand(
    string Name,
    List<string> Permissions,
    Guid? TenantId,
    TenantType TenantType,
    Guid CreatedBy) : IRequest<ErrorOr<MutationResult>>;
