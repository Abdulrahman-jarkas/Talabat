using ErrorOr;
using MediatR;
using Talabat.Accounts.Domain.AccountAggregate.ValueObjects;

namespace Talabat.Accounts.Application.Account.Commands.AddAccount;

internal record AddAccountCommand(
    Guid UserId,
    string Name,
    string Email,
    Guid? TenantId,
    TenantType TenantType,
    List<Guid> RoleIds,
    Guid AssignedBy) : IRequest<ErrorOr<Guid>>;
