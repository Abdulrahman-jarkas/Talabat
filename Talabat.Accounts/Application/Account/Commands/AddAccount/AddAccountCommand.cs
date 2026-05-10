using ErrorOr;
using MediatR;
using Talabat.SharedKernal.Authorization;

namespace Talabat.Accounts.Application.Account.Commands.AddAccount;

internal record AddAccountCommand(
    Guid UserId,
    Guid? TenantId,
    TenantType TenantType,
    List<Guid> RoleIds,
    Guid AssignedBy) : IRequest<ErrorOr<MutationResult>>;
