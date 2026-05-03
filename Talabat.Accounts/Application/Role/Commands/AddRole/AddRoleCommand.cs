using ErrorOr;
using MediatR;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;

namespace Talabat.Accounts.Application.Role.Commands.AddRole;

internal record AddRoleCommand(
    string Name,
    List<string> Permissions,
    Guid? TenantId,
    TenantType TenantType,
    Guid CreatedBy) : IRequest<ErrorOr<Guid>>;
